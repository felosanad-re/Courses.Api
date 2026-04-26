using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Courses.Api.Helper.EmailSend
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("felosanad@gmail.com", "yqcy hvle yhpq kflb")
            };

            return client.SendMailAsync(
                new MailMessage(from: "felosanad@gamil.com",
                                to: email,
                                subject,
                                htmlMessage
                                )
                { IsBodyHtml = true });
        }
    }
}
