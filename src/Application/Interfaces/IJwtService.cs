namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateEmployeeToken(Guid employeeId, string document, string email);
}