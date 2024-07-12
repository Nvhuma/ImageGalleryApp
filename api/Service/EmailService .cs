using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using api.Interfaces;

namespace api.Service;

public class EmailService : IEmailService
{
   
    
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

        /// <summary>
        /// Sends an email with the specified subject and message to the provided recipient.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The message body of the email.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                // Create the email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromAddress, _fromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                // Send the email asynchronously
                await _smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (logging service could be injected similarly to configuration)
                _logger.LogError(ex, $"Error sending email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sends a password reset email with a reset link to the specified recipient.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="resetLink">The password reset link to be included in the email.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            // Customize the subject and message for the password reset email
            string subject = "Password Reset";
            string message = $"To reset your password, please click the following link: {resetLink}";

            // Use the SendEmailAsync method to send the email
            return await SendEmailAsync(toEmail, subject, message);
        }
}
