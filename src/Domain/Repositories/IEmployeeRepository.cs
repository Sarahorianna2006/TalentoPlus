using Domain.Entities;

namespace Domain.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByDocumentAsync(string document);
    Task AddAsync(Employee employee);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task DeleteAsync(Employee employee);
    Task SaveChangesAsync();
}