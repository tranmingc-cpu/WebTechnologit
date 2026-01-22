using System.Net;
using System.Net.Mail;

namespace WebApplication10.Services
{
    public class EmailSenderService
    {
        public bool SendMail(string to, string subject, string body)
        {
            try
            {
                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(
                        "highteckcinema@gmail.com",
                        "hpbxbegnrbwgdzur"
                    ),
                    EnableSsl = true
                };

                var mail = new MailMessage
                {
                    From = new MailAddress("highteckcinema@gmail.com", "Tech Store"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(to);
                smtp.Send(mail);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
