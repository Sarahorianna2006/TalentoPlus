using Application.Interfaces;
using Application.Interfaces.Excel;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IExcelEmployeeReader _excelReader;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IProgramRepository _programRepo;

    public EmployeesController(
        IExcelEmployeeReader excelReader,
        IEmployeeRepository employeeRepo,
        IProgramRepository programRepo)
    {
        _excelReader = excelReader;
        _employeeRepo = employeeRepo;
        _programRepo = programRepo;
    }

    // IMPORT 
    [Consumes("multipart/form-data")]
    [HttpPost("import")]
    public async Task<IActionResult> ImportStudents([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Debe subir un archivo Excel.");

        var results = new List<object>();

        using var stream = file.OpenReadStream();
        var rows = await _excelReader.Read(stream); 

        foreach (var row in rows)
        {
            // Errores del lector Excel
            if (!string.IsNullOrEmpty(row.Error))
            {
                results.Add(new
                {
                    row.Document,
                    row.Name,
                    row.LastName,
                    row.Email,
                    Error = row.Error
                });
                continue;
            }

            // Validar documento único
            var existing = await _employeeRepo.GetByDocumentAsync(row.Document);
            if (existing != null)
            {
                results.Add(new
                {
                    row.Document,
                    row.Name,
                    row.LastName,
                    row.Email,
                    Error = "Documento ya existe"
                });
                continue;
            }

            // Crear empleado 
            var newEmployee = new Employee(
                row.Document,
                row.Name,
                row.LastName,
                row.DateOfBirth,
                row.Address,
                row.Phone,
                row.Email,
                row.Post,
                row.Salary,
                row.EntryDate,
                row.State,
                row.EducationalLevel,
                row.ProfessionalProfile,
                row.Department,
                programId: null
            );

            await _employeeRepo.AddAsync(newEmployee);
            await _employeeRepo.SaveChangesAsync();

            results.Add(new
            {
                row.Document,
                row.Name,
                row.LastName,
                row.Email,
                Success = true
            });
        }

        return Ok(results);
    }

    // CRUD BÁSICO 

    // 1. GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _employeeRepo.GetAllAsync();
        return Ok(students);
    }

    // 2. GET BY DOCUMENT
    [HttpGet("{document}")]
    public async Task<IActionResult> GetByDocument(string document)
    {
        var student = await _employeeRepo.GetByDocumentAsync(document);

        if (student == null)
            return NotFound("Empleado no encontrado.");

        return Ok(student);
    }

    // 3. POST (crear manualmente)
    [HttpPost]
    public async Task<IActionResult> Create(Employee employee)
    {
        await _employeeRepo.AddAsync(employee);
        await _employeeRepo.SaveChangesAsync();
        return Ok(employee);
    }

    // 4. PUT (actualizar por documento)
    [HttpPut("{document}")]
    public async Task<IActionResult> Update(string document, Employee input)
    {
        var student = await _employeeRepo.GetByDocumentAsync(document);

        if (student == null)
            return NotFound("Empleado no encontrado.");

        student.UpdateContact(input.Email, input.Phone);

        await _employeeRepo.SaveChangesAsync();

        return Ok(student);
    }

    // 5. DELETE
    [HttpDelete("{document}")]
    public async Task<IActionResult> Delete(string document)
    {
        var employee = await _employeeRepo.GetByDocumentAsync(document);

        if (employee == null)
            return NotFound("Empleado no encontrado.");

        await _employeeRepo.DeleteAsync(employee);
        await _employeeRepo.SaveChangesAsync();

        return Ok("Empleado eliminado.");
    }
}
