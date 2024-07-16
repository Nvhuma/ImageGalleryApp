using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Interfaces
{
    public interface IEmailService
    {
        
    // Sends an email with the specified subject and message to the provided recipient.
    // <param name="toEmail">The recipient's email address.</param>
    // <param name="subject">The subject of the email.</param>
    // <param name="message">The message body of the email.</param>
    // <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
    Task SendEmailAsync(string email, string subject, string message);
   
    // Sends a password reset email with a reset link to the specified recipient.
    
    // <param name="toEmail">The recipient's email address.</param>
    // <param name="resetLink">The password reset link to be included in the email.</param>
    // <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
    Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}