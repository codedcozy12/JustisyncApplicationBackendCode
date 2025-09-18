using System.Net.Mail;
using System.Net;
using Application.Interfaces.ExternalService;
using Microsoft.Extensions.Configuration;

namespace Infrastructures.Service
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
            var smtpServer = "smtp.gmail.com";
            var port = 587;  
            var senderName = _configuration.GetSection("EmailConfiguration")["SenderName"]!;
            var senderEmail = _configuration.GetSection("EmailConfiguration")["Email"]!;
            var password = _configuration.GetSection("EmailConfiguration")["Password"]!; 

            using var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject =  subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }
    }
}