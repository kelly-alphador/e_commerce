using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;

namespace e_commerce.Services
{
    public class EmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["EmailSettings:Host"])
            {
                Port = int.Parse(ConfigurationManager.AppSettings["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["EmailSettings:Email"],
                    ConfigurationManager.AppSettings["EmailSettings:Password"]
                ),
                EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EmailSettings:EnableSSL"]),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["EmailSettings:Email"]),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}