using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seed;

public static class ProgramSeeder
{
    public static async Task SeedAsync(TalentoPlusDbContext db)
    {
        // Si ya existen programas, no hacemos nada
        if (await db.Programs.AnyAsync())
            return;

        var programs = new List<Program>
        {
            new Program("SIS", "Ingeniería de Sistemas"),
            new Program("ADM", "Administración de Empresas"),
            new Program("CON", "Contaduría Pública"),
            new Program("ENF", "Enfermería"),
            new Program("DER", "Derecho"),
            new Program("PSI", "Psicología")
        };

        await db.Programs.AddRangeAsync(programs);
        await db.SaveChangesAsync();
    }
}