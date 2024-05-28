using System.Net.Mail;

namespace exampProject.Repository
{
    public class EmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            var message = new MailMessage();
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Send(message);
            }
        }
    }
}
