using Application.Interfaces.Excel;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

public class MailtrapEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public MailtrapEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string fullName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("EduTrack", "no-reply@edutrack.com"));
        message.To.Add(new MailboxAddress(fullName, toEmail));
        message.Subject = "Bienvenido a EduTrack";

        message.Body = new TextPart("html")
        {
            Text = $@"
                <h2>Hola {fullName},</h2>
                <p>Tu registro en <strong>EduTrack</strong> ha sido completado exitosamente.</p>
                <p>Gracias por usar el sistema.</p>"
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            "sandbox.smtp.mailtrap.io",
            587,
            MailKit.Security.SecureSocketOptions.StartTls
        );

        await client.AuthenticateAsync(
            _config["Mailtrap:User"],
            _config["Mailtrap:Pass"]
        );

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
    
}