using Application.Interfaces;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class AiQuestionDto
{
    public string Question { get; set; } = "";
}

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly IAiAnalysisService _ai;
    private readonly IEmployeeRepository _repo;

    public AiController(IAiAnalysisService ai, IEmployeeRepository repo)
    {
        _ai = ai;
        _repo = repo;
    }

    [HttpPost("query")]
    public async Task<IActionResult> Query([FromBody] AiQuestionDto body)
    {
        if (body == null || string.IsNullOrWhiteSpace(body.Question))
            return BadRequest("La pregunta no puede estar vacía.");

        // IA interpreta la pregunta
        var instruction = await _ai.AnalyzeQuestion(body.Question);

        // Ejecutamos la lógica con la BD real
        var answer = await _ai.ResolveInstruction(instruction, _repo);

        return Ok(new { Answer = answer });
    }
}