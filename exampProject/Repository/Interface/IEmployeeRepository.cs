using exampProject.Models;

namespace exampProject.Repository.Interface
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        public Employee Add(Employee employee);
        public Task<Employee> AddAsync(Employee employee);
        public Task<Employee> Edit(Employee employee);
        public Task<Employee> Delete(Employee employee);
        public Employee GetById(int id);

    }
}
