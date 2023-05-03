using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using EducationSayt.Services.Interfaces;
using EducationSayt.Helpers;

namespace EducationSayt.Services
{
    public class EmailService:IEmailService
    {
    
        private readonly EmailSetting _emailSettings;

        public EmailService(IOptions<EmailSetting> emailSetting)
        {

            _emailSettings = emailSetting.Value;
        }

        public void Send(string to, string subject, string html, string from = null)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _emailSettings.FromAddress));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
           
            using var smtp = new SmtpClient();
            //smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            smtp.Connect(_emailSettings.Server, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.UserName, _emailSettings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);



        }
    }
}
