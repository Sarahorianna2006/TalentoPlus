using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class TalentoPlusDbContextFactory : IDesignTimeDbContextFactory<TalentoPlusDbContext>
{
    public TalentoPlusDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TalentoPlusDbContext>();

        var connectionString =
            "Server=localhost;Port=3306;Database=talentoplusdb;User=root;Password=Qwe.123*;";

        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
        );

        return new TalentoPlusDbContext(optionsBuilder.Options);
    }
}