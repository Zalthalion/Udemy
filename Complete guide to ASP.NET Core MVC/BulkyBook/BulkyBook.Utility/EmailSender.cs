using Microsoft.AspNetCore.Identity.UI.Services;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Not a fan of this dummy implementation tho
            return Task.CompletedTask;
        }
    }
}
