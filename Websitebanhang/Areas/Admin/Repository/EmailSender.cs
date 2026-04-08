using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Websitebanhang.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("demoemailweb202@gmail.com", "knewjmslodtiuejn"),
                EnableSsl = true,
            };
            
            return smtpClient.SendMailAsync(
                new MailMessage(from: "demoemailweb202@gmail.com", to: email, subject: subject, body: message));
        }
    }
}
