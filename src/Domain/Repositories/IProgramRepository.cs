using Domain.Entities;

namespace Domain.Repositories;

public interface IProgramRepository
{
    Task<Program?> GetByCodeAsync(string code);
    Task<IEnumerable<Program>> GetAllAsync();
}