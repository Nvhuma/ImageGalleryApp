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

   

}
