using System.Net.Mail;
using System.Text;

namespace LenesKlinik.Core.ApplicationServices.Implementation
{
    public class EmailService : IEmailService
    {
        private SmtpClient _client;

        public EmailService()
        {
            _client = GetClient();
        }


        public void SendMail(string emailTo, string title, string body)
        {
            MailMessage mm = new MailMessage("makeklinik@gmail.com", emailTo, title, body)
            {
                BodyEncoding = Encoding.UTF8,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };
            _client.Send(mm);
            mm.Dispose();
        }


        /// <summary>
        /// Create a new SmptClient 
        /// </summary>
        /// <returns></returns>
        private SmtpClient GetClient()
        {
            return new SmtpClient()
            {
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("makeklinik@gmail.com", "SuperGodtPass123*")
            };
        }
    }
}