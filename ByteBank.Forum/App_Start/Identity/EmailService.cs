using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ByteBank.Forum.App_Start.Identity
{
    public class EmailService : IIdentityMessageService
    {
        private readonly string _emailOrigin = ConfigurationManager.AppSettings["emailService:_emailOrigin"];
        private readonly string _emailPassword = ConfigurationManager.AppSettings["emailService:_emailPassword"];

        public async Task SendAsync(IdentityMessage message)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_emailOrigin);

                mailMessage.Subject = message.Subject;
                mailMessage.To.Add(message.Destination);
                mailMessage.Body = message.Body;

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(_emailOrigin, _emailPassword);

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;

                    smtpClient.Timeout = 20_000;

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }
    }
}