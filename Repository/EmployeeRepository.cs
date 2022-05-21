using Dapper;
using DapperDemo.Data;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperDemo.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnection db;
        public EmployeeRepository(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public Employee Add(Employee employee)
        {
            var sql = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
                        +"SELECT CAST(SCOPE_IDENTITY() as int); ";
            #region First Way
            //var id = db.Query<int>(sql, new
            //{
            //    @Name = Employee.Name,
            //    @Address = Employee.Address,
            //    @City = Employee.City,
            //    @State = Employee.State,
            //    @PostalCode = Employee.PostalCode,
            //}).Single(); 
            #endregion

            #region Second Way 
            //DP smart enough to identify columns, if all colums names are same as that of prop names
            var id = db.Query<int>(sql, employee).Single(); 
            #endregion

            employee.EmployeeId = id;
            return employee;
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            var sql = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
                       + "SELECT CAST(SCOPE_IDENTITY() as int); ";
            
           
            var id = await db.QueryAsync<int>(sql, employee);
            
            employee.EmployeeId = id.Single();
            return employee;
        }

        public Employee Find(int id)
        {
            var sql = "SELECT * FROM Employees WHERE EmployeeId = @Id";
            return db.Query<Employee>(sql, new { id }).Single();
        }
        public List<Employee> GetAll()
        {
            var sql = "SELECT * FROM Employees";
            return db.Query<Employee>(sql).ToList();
        }
        public void Remove(int id)
        {
            var sql = "DELETE FROM Employees WHERE EmployeeId = @Id";
            db.Execute(sql, new {id}); //Since param and passed value are same no need of @id = id setting.
        }
        public Employee Update(Employee employee)
        {
            var sql = "UPDATE Employees SET Name = @Name, Title = @Title, Email = @Email, Phone = @Phone," 
                        +"CompanyId = @CompanyId WHERE EmployeeId = @EmployeeId";
            db.Execute(sql, employee);
            return employee;
        }
    }
}
