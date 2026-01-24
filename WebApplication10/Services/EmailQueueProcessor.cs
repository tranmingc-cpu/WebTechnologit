using System.Linq;
using WebApplication10.DAO;
using WebApplication10.Models;

namespace WebApplication10.Services
{
    public class EmailQueueProcessor
    {
        private readonly EmailQueueDAO _emailQueueDao;
        private readonly EmailSenderService _emailSender;

        public EmailQueueProcessor(TechStoreDBEntities db)
        {
            _emailQueueDao = new EmailQueueDAO(db);
            _emailSender = new EmailSenderService();
        }

        public void Process(int take = 10)
        {
            var emails = _emailQueueDao.GetPendingEmails(take);

            foreach (var mail in emails)
            {
                var success = _emailSender.SendMail(
                    mail.ToEmail,
                    mail.Subject,
                    mail.Body
                );

                if (success)
                {
                    _emailQueueDao.MarkAsSent(mail.EmailQueueId);
                }
                else
                {
                    _emailQueueDao.MarkAsFailed(mail.EmailQueueId);
                }
            }
        }
    }
}
