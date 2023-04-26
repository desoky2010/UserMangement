using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using UserMangement.Helpers;

namespace UserMangement.Services
{
    public class MailService : IMailService
    {
        private readonly Mail _setting;

        public MailService(IOptions<Mail> setting)
        {
            _setting = setting.Value;
        }

        public async Task SendEmail(string mailto, string subject, string body)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_setting.Email),
                Subject = subject,
            };
            email.To.Add(MailboxAddress.Parse(mailto));
            var builder = new BodyBuilder();
            builder.HtmlBody= body;
            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_setting.DisplayName , _setting.Email));

            using var smtp = new SmtpClient();
            smtp.Connect(_setting.Host, _setting.Port,MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_setting.Email, _setting.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
