using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/admin/import")]
public class AdminImportController : ControllerBase
{
    private readonly ImportEmployeesUseCase _useCase;

    public AdminImportController(ImportEmployeesUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Archivo vacío");

        using var stream = file.OpenReadStream();
        var result = await _useCase.Execute(stream);

        return Ok(result);
    }
}