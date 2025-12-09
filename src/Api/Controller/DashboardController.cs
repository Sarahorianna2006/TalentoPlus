using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controller;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly TalentoPlusDbContext _db;

    public DashboardController(TalentoPlusDbContext db)
    {
        _db = db;
    }

    // MÉTRICAS
    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics()
    {
        var total = await _db.Employees.CountAsync();
        var activos = await _db.Employees.CountAsync(s => s.State == "Activo");

        var porPrograma = await _db.Employees
            .Include(s => s.Program)
            .GroupBy(s => s.Program != null ? s.Program.Name : "Sin programa")
            .Select(g => new
            {
                Program = g.Key,
                Total = g.Count()
            })
            .ToListAsync();

        return Ok(new
        {
            total,
            activos,
            porPrograma
        });
    }

}