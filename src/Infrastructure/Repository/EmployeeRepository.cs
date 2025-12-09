using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly TalentoPlusDbContext _db;

    public EmployeeRepository(TalentoPlusDbContext db)
    {
        _db = db;
    }

    // -------------------------------------------
    // OBTENER POR DOCUMENTO
    // -------------------------------------------
    public async Task<Employee?> GetByDocumentAsync(string document)
    {
        return await _db.Employees
            .Include(e => e.Program)
            .FirstOrDefaultAsync(e => e.Document == document);
    }

    // -------------------------------------------
    // LISTAR TODOS
    // -------------------------------------------
    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _db.Employees
            .Include(e => e.Program)
            .ToListAsync();  // Esto está bien, EF lo retorna como List<Employee> que implementa IEnumerable
    }

    // -------------------------------------------
    // AGREGAR
    // -------------------------------------------
    public async Task AddAsync(Employee employee)
    {
        await _db.Employees.AddAsync(employee);
    }

    // -------------------------------------------
    // ELIMINAR
    // -------------------------------------------
    public async Task DeleteAsync(Employee employee)
    {
        _db.Employees.Remove(employee);
    }

    // -------------------------------------------
    // GUARDAR CAMBIOS
    // -------------------------------------------
    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}