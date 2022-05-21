using DapperDemo.Models;
using DapperDemo.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DapperDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBonusRepository _bonusRepo;

        public HomeController(ILogger<HomeController> logger, IBonusRepository bonusRepo)
        {
            _logger = logger;
            _bonusRepo  = bonusRepo;
        }

        public IActionResult Index()
        {
            var companies = _bonusRepo.GetAllCompaniesWithEmployees();
            return View(companies);
        }   
        
        public IActionResult AddTestRecords()
        {
            Company company = new Company()
            {
                Name = "Test" + Guid.NewGuid().ToString(),
                Address = "test address",
                City = "test city",
                PostalCode = "test postalCode",
                State = "test state",
                Employees = new List<Employee>()
            };

            company.Employees.Add(new Employee()
            {
                Email = "test Email",
                Name = "Test Name " + Guid.NewGuid().ToString(),
                Phone = " test phone",
                Title = "Test Manager"
            });

            company.Employees.Add(new Employee()
            {
                Email = "test Email 2",
                Name = "Test Name 2" + Guid.NewGuid().ToString(),
                Phone = " test phone 2",
                Title = "Test Manager 2"
            });
            //_bonusRepo.AddTestCompanyWithEmployees(company);
            _bonusRepo.AddTestCompanyWithEmployeesWithTransaction(company);
            return RedirectToAction(nameof(Index));   
        }

        public IActionResult RemoveTestRecords()
        {
            var companyIdsToRemove = _bonusRepo.FilterCompanyByName("Test").Select(x=>x.CompanyId).ToArray();
            _bonusRepo.RemoveRange(companyIdsToRemove);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}