using System.Net;
using System.Net.Mail;

namespace MVC_Project.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"] 
                ?? Environment.GetEnvironmentVariable("EMAIL_SENDER") 
                ?? "";
            var senderPassword = _configuration["EmailSettings:SenderPassword"] 
                ?? Environment.GetEnvironmentVariable("EMAIL_PASSWORD") 
                ?? "";
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var smtpPort = int.TryParse(_configuration["EmailSettings:SmtpPort"], out int port) ? port : 587;

            // If credentials are not configured, skip sending safely without crashing
            if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(senderPassword))
            {
                return;
            }

            using var smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}