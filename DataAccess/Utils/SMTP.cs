using System.Net.Mail;

namespace DataAccess.Utils
{
    public class SMTP
    {
        public const string HOST = "smtp.gmail.com";
        public const int PORT = 587;
        public const string EMAIL = "nguyntrng234@gmail.com";
        public const string PASSWORD = "kubbxerhqhoazngp";

        public static SMTP Instance { get; } = new(HOST, PORT, EMAIL, PASSWORD);

        private SmtpClient client;

        public SMTP(string host, int port, string email, string password)
        {
            client = new SmtpClient(host, port);

            client.Credentials = new System.Net.NetworkCredential(email, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
        }

        public void Send(string subject, string body, params string[] receivers)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("trantungmr11@gmail.com", "Cinema System");
            foreach (var receiver in receivers)
            {
                mail.To.Add(new MailAddress(receiver));
            }

            mail.Subject = subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = body;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            client.Send(mail);
        }
    }
}