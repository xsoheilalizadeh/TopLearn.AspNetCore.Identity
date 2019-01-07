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
            var message = new MailMessage("no-reply@toplearn.com", email, subject, htmlMessage)
            {
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8
            };

            var smtpClient = new SmtpClient("toplearn.com")
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("no-reply@toplearn.com", "Top@123#"),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            return smtpClient.SendMailAsync(message);
        }
    }
}