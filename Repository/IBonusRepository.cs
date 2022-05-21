using DapperDemo.Models;

namespace DapperDemo.Repository
{
    public interface IBonusRepository
    {
        List<Employee> GetEmployeeWithCompany(int id);
        Company GetCompanyWithEmployees(int id);
        List<Company> GetAllCompaniesWithEmployees();

        void AddTestCompanyWithEmployees(Company company);
        void AddTestCompanyWithEmployeesWithTransaction(Company company);
        void RemoveRange(int[] companyIds);

        List<Company> FilterCompanyByName(string name);
    }
}
