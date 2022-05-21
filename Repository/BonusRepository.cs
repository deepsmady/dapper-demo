using Dapper;
using DapperDemo.Data;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace DapperDemo.Repository
{
    public class BonusRepository : IBonusRepository
    {

        private readonly IDbConnection db;
        public BonusRepository(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public void AddTestCompanyWithEmployees(Company company)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                        + "SELECT CAST(SCOPE_IDENTITY() as int); ";

            var id = db.Query<int>(sql, company).Single();
            company.CompanyId = id;

            //foreach (var employee in company.Employees)
            //{
            //    employee.CompanyId = company.CompanyId;
            //    var sql1 = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
            //           + "SELECT CAST(SCOPE_IDENTITY() as int); ";


            //    var id1 = db.Query<int>(sql1, employee).Single();
            //    employee.EmployeeId = id1;  
            //}

            //Bulk Insert
            company.Employees.Select(e => { e.CompanyId = id; return e; }).ToList();
            var sqlEmp = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
                   + "SELECT CAST(SCOPE_IDENTITY() as int); ";
            db.Execute(sqlEmp, company.Employees);
        }

        public void AddTestCompanyWithEmployeesWithTransaction(Company company)
        {
            //Transaction like Unit of work
            using (var transaction = new TransactionScope())
            {
                try
                {
                    var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                                        + "SELECT CAST(SCOPE_IDENTITY() as int); ";

                    var id = db.Query<int>(sql, company).Single();
                    company.CompanyId = id;

                    company.Employees.Select(e => { e.CompanyId = id; return e; }).ToList();
                    var sqlEmp = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
                           + "SELECT CAST(SCOPE_IDENTITY() as int); ";
                    db.Execute(sqlEmp, company.Employees);

                    transaction.Complete(); 
                }
                catch (Exception ex)
                {

                }
            }
        }



        public List<Company> GetAllCompaniesWithEmployees()
        {
            var sql = @"select C.*, E.* from Employees E
	                    join Companies C on E.CompanyId = C.CompanyId";

            var compDict = new Dictionary<int, Company>();

            var company = db.Query<Company, Employee, Company>(sql, (c, e) =>
            {
                if (!compDict.TryGetValue(c.CompanyId, out var currentCompany))
                {
                    currentCompany = c;
                    compDict.Add(c.CompanyId, currentCompany);
                }
                currentCompany.Employees.Add(e);
                return currentCompany;
            }, splitOn: "EmployeeId");

            return company.Distinct().ToList();
        }

        public Company GetCompanyWithEmployees(int id)
        {
            var p = new
            {
                CompanyId = id
            };

            var sql = "select * from Companies where CompanyId = @CompanyId;"
                    + "select * from Employees where CompanyId = @CompanyId;";

            Company company;

            using (var lists = db.QueryMultiple(sql, p))
            {
                company = lists.Read<Company>().ToList().FirstOrDefault();
                company.Employees = lists.Read<Employee>().ToList();
            }

            return company;
        }

        public List<Employee> GetEmployeeWithCompany(int id)
        {
            var sql = @"select E.*, C.* from Employees E
	                    join Companies C on E.CompanyId = C.CompanyId";

            if (id != 0)
            {
                sql += " where E.CompanyId = @Id ";
            }

            var employee = db.Query<Employee, Company, Employee>(sql, (e, c) =>
            {
                e.Company = c;
                return e;
            }, new { id }, splitOn: "CompanyId");

            return employee.ToList();
        }
        public void RemoveRange(int[] companyIds)
        {
            db.Query("Delete from Companies where CompanyId IN @companyIds", new { companyIds });
        }

        public List<Company> FilterCompanyByName(string name)
        {
            var companies = db.Query<Company>("select * from Companies where Name like '%' + @name+ '%' ", new { name });
            return companies.ToList();
        }
    }
}
