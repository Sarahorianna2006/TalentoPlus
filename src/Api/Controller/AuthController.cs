using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Interfaces.Excel;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IProgramRepository _programRepo;
    private readonly IJwtService _jwt;
    private readonly IEmailService _email;

    public AuthController(
        IEmployeeRepository employeeRepo,
        IProgramRepository programRepo,
        IJwtService jwt,
        IEmailService email) 
    {
        _employeeRepo = employeeRepo;
        _programRepo = programRepo;
        _jwt = jwt;
        _email = email; 
    }
    
    //   1. Autoregistro
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterEmployeeDto dto)
    {
        var program = await _programRepo.GetByCodeAsync(dto.ProgramCode);
        if (program == null)
            return BadRequest("ProgramCode inválido.");

        var existing = await _employeeRepo.GetByDocumentAsync(dto.Document);
        if (existing != null)
            return BadRequest("Documento ya registrado.");

        var newEmployee = new Employee(
            dto.Document,
            dto.Name,
            dto.LastName,
            dto.DateOfBirth,
            dto.Address,
            dto.Phone,
            dto.Email,
            dto.Post,
            dto.Salary,
            dto.EntryDate,
            dto.State,
            dto.EducationalLevel,
            dto.ProfessionalProfile,
            dto.Department,
            program.Id
        );

        await _employeeRepo.AddAsync(newEmployee);
        await _employeeRepo.SaveChangesAsync();
        
        //  ENVIAR CORREO DE CONFIRMACIÓN (MAILTRAP)
        await _email.SendEmailAsync(newEmployee.Email, $"{newEmployee.Name} {newEmployee.LastName}");


        return Ok("Estudiante registrado correctamente.");
    }
    
    //   2. Login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var employee = await _employeeRepo.GetByDocumentAsync(dto.Document);

        if (employee == null || employee.Email != dto.Email)
            return Unauthorized("Credenciales inválidas.");

        var token = _jwt.GenerateEmployeeToken(
            employee.Id,
            employee.Document,
            employee.Email
        );

        return Ok(new LoginResponseDto
        {
            Token = token,
            FullName = $"{employee.LastName}",
            Email = employee.Email
        });
    }
    
    //   3. Perfil (JWT requerido)
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var id = User.FindFirst("studentId")?.Value;
        var document = User.FindFirst("document")?.Value;
        var email = User.FindFirst("email")?.Value;

        return Ok(new
        {
            EmployeeId = id,
            Document = document,
            Email = email
        });
    }
}