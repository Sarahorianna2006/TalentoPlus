using Application.DTOs;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProgramsController : ControllerBase
{
    private readonly IProgramRepository _programRepo;

    public ProgramsController(IProgramRepository programRepo)
    {
        _programRepo = programRepo;
    }

    // GET: api/programs
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var programs = await _programRepo.GetAllAsync();

        var dtoList = programs.Select(p => new ProgramDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name
        });

        return Ok(dtoList);
    }

    // GET: api/programs/{code}
    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var program = await _programRepo.GetByCodeAsync(code);

        if (program == null)
            return NotFound($"Programa con código '{code}' no encontrado.");

        var dto = new ProgramDto
        {
            Id = program.Id,
            Code = program.Code,
            Name = program.Name
        };

        return Ok(dto);
    }
}