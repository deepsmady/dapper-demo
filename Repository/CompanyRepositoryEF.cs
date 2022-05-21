using DapperDemo.Data;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;

namespace DapperDemo.Repository
{
    public class CompanyRepositoryEF : ICompanyRepository
    {
        private ApplicationDbContext _db;

        public CompanyRepositoryEF(ApplicationDbContext db)
        {
            _db = db;
        }
        public Company Add(Company company)
        {
            _db.Companies.Add(company);
            _db.SaveChanges();
            return company;
        }

        public Company Find(int id)
        {
            return _db.Companies.FirstOrDefault(c => c.CompanyId == id);
        }

        public List<Company> GetAll()
        {
            return _db.Companies.ToList();
        }

        public void Remove(int id)
        {
            var company = _db.Companies.FirstOrDefault(c => c.CompanyId == id);
            _db.Companies.Remove(company);
            _db.SaveChanges();
        }

        public Company Update(Company company)
        {
            _db.Companies.Update(company);
            _db.SaveChanges();
            return company;
        }
    }
}
