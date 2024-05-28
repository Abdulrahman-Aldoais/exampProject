using exampProject.DBContext;
using exampProject.Models;
using exampProject.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace exampProject.Repository.Implemntation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DbaceContext _dbaceContext;
        public EmployeeRepository(DbaceContext dbaceContext)
        {
            _dbaceContext = dbaceContext;
        }
        public Employee Add(Employee employee)
        {
            _dbaceContext.Entry(employee).State = EntityState.Added;
            _dbaceContext.Employees.Add(employee);
            _dbaceContext.SaveChanges();
            return employee;
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            _dbaceContext.Entry(employee).State |= EntityState.Added;
            await _dbaceContext.Employees.AddAsync(employee);
            await _dbaceContext.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> Delete(Employee employee)
        {
            _dbaceContext.Entry(employee).State &= ~EntityState.Deleted;
            _dbaceContext.Employees.Remove(employee);
            _dbaceContext.SaveChanges();
            return employee;
        }

        public async Task<Employee> Edit(Employee employee)
        {
            _dbaceContext.Entry(employee).State = EntityState.Modified;
            _dbaceContext.Employees.Update(employee);
            await _dbaceContext.SaveChangesAsync();
            return employee;
        }


        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var allEmployee = await _dbaceContext.Employees
                                                 .Include(d => d.Department).ToListAsync();
            return allEmployee;
        }

        public Employee GetById(int id)
        {

            var employee = _dbaceContext.Employees.FirstOrDefault(c => c.Id == id);
            return employee;

        }
    }
}
