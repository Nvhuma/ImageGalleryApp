using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using api.Interfaces;

namespace api.Service
;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromAddress;

    public EmailService(IConfiguration configuration)
    {
        // Configure the SMTP client based on your application's settings
        _smtpClient = new SmtpClient
        {
            Host = configuration["Smtp:Host"],
            Port = int.Parse(configuration["Smtp:Port"]),
            EnableSsl = true,
            Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"])
        };
        _fromAddress = configuration["Smtp:FromAddress"];
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromAddress),
                Subject = subject,
                Body = message,
                IsBodyHtml = false // Set to true if your message contains HTML
            };
            mailMessage.To.Add(toEmail);

            await _smtpClient.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (logging service could be injected similarly to configuration)
            Console.WriteLine("Error sending email: " + ex.Message);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        // You can customize the subject and message here
        string subject = "Password Reset";
        string message = $"To reset your password, please click the following link: {resetLink}";

        return await SendEmailAsync(toEmail, subject, message);
    }
}
