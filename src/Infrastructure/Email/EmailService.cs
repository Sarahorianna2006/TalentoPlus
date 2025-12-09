using System.Net;
using System.Net.Mail;
using Application.Interfaces.Excel;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string fullName)
    {
        var host = _config["Smtp:Host"];
        var port = int.Parse(_config["Smtp:Port"]);
        var username = _config["Smtp:Username"];
        var password = _config["Smtp:Password"];
        var from = _config["Smtp:From"];

        var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        var mail = new MailMessage( toEmail, fullName);

        await client.SendMailAsync(mail);
    }
}