using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace App.Services.Identity
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MailMessage("example@mail.com", email, subject, htmlMessage)
            {
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8
            };

            var smtpClient = new SmtpClient("example.com")
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("username", "password"),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            return smtpClient.SendMailAsync(message);
        }
    }
}