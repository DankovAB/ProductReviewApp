using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeKit;
using ProductReviewApp.Persistence.Models;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ProductReviewApp.Application.UserNotification
{
    public class EmailNotification: IUserNotification
    {
        private readonly EmailActualizeNotificationSettings _notificationSettings;
        private readonly SmtpSettings _smtpSettings;

        public EmailNotification(
            IOptions<EmailActualizeNotificationSettings> emailNotificationSettings,
            IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
            _notificationSettings = emailNotificationSettings.Value;
        }

        public async Task NotifyActualizeProduct(Product product)
        {
            var bodyBuilder = new BodyBuilder {TextBody = $"{product.Asin} product is actualized"};

            var message = new MimeMessage();

            var from = new MailboxAddress("no-reply@datahawk.co", _notificationSettings.FromUser);
            message.From.Add(from);

            var to = MailboxAddress.Parse(_notificationSettings.ToUser);
            message.To.Add(to);

            message.Subject = _notificationSettings.Subject;
            message.Body = bodyBuilder.ToMessageBody();

            await SendMail(message);
        }

        private async Task SendMail(MimeMessage email)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, _smtpSettings.UseSll);
                await client.AuthenticateAsync(_smtpSettings.User, _smtpSettings.Password);

                await client.SendAsync(email);
            }
        }
    }
}
