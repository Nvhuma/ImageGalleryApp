using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using api.Interfaces;

namespace api.Service;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly SmtpClient _smtpClient;
    private readonly string _fromAddress;

    private readonly string _fromName;
    private readonly ILogger<EmailService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// Configures the SMTP client using the application's settings.
    /// </summary>
    /// <param name="configuration">The configuration object containing SMTP settings.</param>
    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {

        _configuration = configuration;
        // Configure the SMTP client based on your application's settings
        _smtpClient = new SmtpClient
        {
            Host = configuration["Smtp:Host"],
            Port = int.Parse(configuration["Smtp:Port"]),
            EnableSsl = true,
            Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"])
        };
        _fromAddress = configuration["Smtp:FromAddress"];
        _fromName = configuration["Smtp:FromName"];
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpHost = _configuration["Smtp:Host"];
        var smtpPortString = _configuration["Smtp:Port"];
        var smtpUsername = _configuration["Smtp:Username"];
        var smtpPassword = _configuration["Smtp:Password"];
        var smtpFrom = _configuration["Smtp:From"];

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPortString) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpFrom))
        {
            throw new ArgumentNullException("SMTp configuration is missing or incomplete.");
        }

        if (!int.TryParse(smtpPortString, out int smtpPort))
        {
            throw new FormatException("SMTP port configuration is not a valid integer.");
        }

        var smtpClient = new SmtpClient(smtpHost)
        {
            // Port = int.Parse(smtpHost),
            Port = smtpPort,
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = true
        };

        // Custom server certificate validation
        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
        {
            // Allow all certificates for development, not recommended for production
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
            {
                foreach (var status in chain.ChainStatus)
                {
                    if (status.Status != X509ChainStatusFlags.NoError && status.Status != X509ChainStatusFlags.UntrustedRoot)
                    {
                        return false;
                    }
                }
            }
            return true;
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpFrom),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not send email to {Email}", email);
            // Log the exception
            throw new InvalidOperationException("Could not send email", ex);
        }
    }

    public Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sends an email with the specified subject and message to the provided recipient.
    /// </summary>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="message">The message body of the email.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
    // public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
    // {
    //     try
    //     {
    //         // Create the email message
    //         var mailMessage = new MailMessage
    //         {
    //             From = new MailAddress(_fromAddress, _fromName),
    //             Subject = subject,
    //             Body = message,
    //             IsBodyHtml = true
    //         };
    //         mailMessage.To.Add(toEmail);

    //         // Send the email asynchronously
    //         await _smtpClient.SendMailAsync(mailMessage);
    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         // Log the exception (logging service could be injected similarly to configuration)
    //         _logger.LogError(ex, $"Error sending email: {ex.Message}");
    //         return false;
    //     }
    // }

    // /// <summary>
    // /// Sends a password reset email with a reset link to the specified recipient.
    // /// </summary>
    // /// <param name="toEmail">The recipient's email address.</param>
    // /// <param name="resetLink">The password reset link to be included in the email.</param>
    // /// <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
    // public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink)
    // {
    //     // Customize the subject and message for the password reset email
    //     string subject = "Password Reset";
    //     string message = $"To reset your password, please click the following link: {resetLink}";

    //     // Use the SendEmailAsync method to send the email
    //     return await SendEmailAsync(toEmail, subject, message);
    // }


}
