namespace Application.Interfaces.Excel;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string fullName);
}