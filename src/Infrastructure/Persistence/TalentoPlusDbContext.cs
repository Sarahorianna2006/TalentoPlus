using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class TalentoPlusDbContext : IdentityDbContext<ApplicationUser>
{
    public TalentoPlusDbContext(DbContextOptions<TalentoPlusDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Program> Programs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .HasIndex(s => s.Document)
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .HasOne(s => s.Program)
            .WithMany(p => p.Employees)
            .HasForeignKey(s => s.ProgramId);

        modelBuilder.Entity<Program>()
            .HasIndex(p => p.Code)
            .IsUnique();
    }
}