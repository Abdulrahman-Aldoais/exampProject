using exampProject.DBContext;
using exampProject.Models;
using exampProject.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace exampProject.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly DbaceContext _dbaceContext;
        public EmployeeController(IEmployeeRepository employeeRepository, DbaceContext dbaceContext)
        {
            _employeeRepository = employeeRepository;
            _dbaceContext = dbaceContext;
        }

        public async Task<IActionResult> AllEmployee(int? departmentId)
        {
            var allDepartment = _dbaceContext.Departments.ToList();
            ViewBag.Departments = allDepartment;
            var employees = await _employeeRepository.GetAllAsync();
            if (departmentId.HasValue)
                employees = employees.Where(e => e.DepartmentId == departmentId.Value);

            return View(employees);
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            var allDepartments = _dbaceContext.Departments.ToList();
            ViewBag.Departments = allDepartments;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                _dbaceContext.Employees.Add(employee);
                await _dbaceContext.SaveChangesAsync();
                return Json(new { success = true, message = "Employee added successfully" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Invalid data", errors = errors });
        }

        public async Task<IActionResult> Filter(int? departmentId)
        {
            IEnumerable<Employee> employees = await _dbaceContext.Employees
                .Include(e => e.Department)
                .Where(e => !departmentId.HasValue || e.DepartmentId == departmentId)
                .ToListAsync();

            return PartialView("_EmployeeTableRows", employees);
        }


    }
}
