using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class ProgramRepository : IProgramRepository
{
    private readonly TalentoPlusDbContext _db;

    public ProgramRepository(TalentoPlusDbContext db)
    {
        _db = db;
    }

    public async Task<Program?> GetByCodeAsync(string code)
    {
        return await _db.Programs.FirstOrDefaultAsync(p => p.Code == code);
    }

    public async Task<IEnumerable<Program>> GetAllAsync()
    {
        return await _db.Programs
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}