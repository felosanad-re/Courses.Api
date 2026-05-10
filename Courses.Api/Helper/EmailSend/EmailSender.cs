using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Courses.Api.Helper.EmailSend
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("felosanad@gmail.com", "tmch typc kpwf zitx")
                };

                await client.SendMailAsync(
                    new MailMessage(from: "felosanad@gmail.com",
                                    to: email,
                                    subject,
                                    htmlMessage)
                    {
                        IsBodyHtml = true
                    });

                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}. Account was created but confirmation email failed.", email);
            }
        }
    }
}
