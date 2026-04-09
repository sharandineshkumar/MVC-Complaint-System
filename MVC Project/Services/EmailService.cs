using System.Net;
using System.Net.Mail;

namespace MVC_Project.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")//connecting to the gmail server 
            {
                Port = 587,
                Credentials = new NetworkCredential("civicportal949@gmail.com", "ihcz fkgl vfhq ecge"),
                EnableSsl = true,  // means the connection is encrypted. Nobody can intercept and read your email credentials while they travel over the internet.
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("civicportal949@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}