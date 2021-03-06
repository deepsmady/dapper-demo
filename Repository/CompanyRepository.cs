using Dapper;
using DapperDemo.Data;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperDemo.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbConnection db;
        public CompanyRepository(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public Company Add(Company company)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                        + "SELECT CAST(SCOPE_IDENTITY() as int); ";
            #region First Way
            //var id = db.Query<int>(sql, new
            //{
            //    @Name = company.Name,
            //    @Address = company.Address,
            //    @City = company.City,
            //    @State = company.State,
            //    @PostalCode = company.PostalCode,
            //}).Single(); 
            #endregion

            #region Second Way 
            //DP smart enough to identify columns, if all colums names are same as that of prop names
            var id = db.Query<int>(sql, company).Single(); 
            #endregion

            company.CompanyId = id;
            return company;
        }
        public Company Find(int id)
        {
            var sql = "SELECT * FROM Companies WHERE CompanyId = @CompanyId";
            return db.Query<Company>(sql, new { @CompanyId = id }).Single();
        }
        public List<Company> GetAll()
        {
            var sql = "SELECT * FROM Companies";
            return db.Query<Company>(sql).ToList();
        }
        public void Remove(int id)
        {
            var sql = "DELETE FROM Companies WHERE CompanyId = @Id";
            db.Execute(sql, new {@id}); //Since param anf passed value are same no need of @id = id setting.
        }
        public Company Update(Company company)
        {
            var sql = "UPDATE Companies SET Name = @Name, Address = @Address, City = @City, State = @State, "
                            +"PostalCode = @PostalCode WHERE CompanyId = @CompanyId";
            db.Execute(sql, company);
            return company;
        }
    }
}
