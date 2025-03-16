using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using WebApp_Identity.Settings;

namespace WebApp_Identity.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            this.smtpSettings = smtpSettings.Value;
        }

        public async Task SendAsync(string emailSender, string sendEmailTo, string emailSubject, string emailBody)
        {
            var message = new MailMessage(emailSender, sendEmailTo, emailSubject, emailBody);

            using (var emailClient = new SmtpClient(this.smtpSettings.Host, this.smtpSettings.Port))
            {
                emailClient.Credentials = new NetworkCredential(this.smtpSettings.Login, this.smtpSettings.MasterKey);
                emailClient.EnableSsl = this.smtpSettings.EnableSSL;
                await emailClient.SendMailAsync(message);
            }
        }
    }
}
