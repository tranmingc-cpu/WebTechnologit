using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication10.Models;

namespace WebApplication10.DAO
{
    public class EmailQueueDAO
    {
        private readonly TechStoreDBEntities _db;

        public EmailQueueDAO()
        {
            _db = new TechStoreDBEntities();
        }

        public EmailQueueDAO(TechStoreDBEntities context)
        {
            _db = context;
        }

        public void Enqueue(EmailQueue email)
        {
            email.Status = 0;       
            email.RetryCount = 0;
            email.CreatedAt = DateTime.Now;

            _db.EmailQueue.Add(email);
            _db.SaveChanges();
        }
        public void Enqueue(
            string toEmail,
            string subject,
            string body,
            string emailType,
            int? subscriberId = null
        )
        {
            var email = new EmailQueue
            {
                ToEmail = toEmail,
                Subject = subject,
                Body = body,
                EmailType = emailType,
                SubscriberId = subscriberId,
                Status = 0,        
                RetryCount = 0,
                CreatedAt = DateTime.Now
            };

            _db.EmailQueue.Add(email);
            _db.SaveChanges();
        }
        public List<EmailQueue> GetPendingEmails(int limit = 10)
        {
            return _db.EmailQueue
                      .Where(e => e.Status == 0)
                      .OrderBy(e => e.CreatedAt)
                      .Take(limit)
                      .ToList();
        }
        public void MarkAsSent(int emailQueueId)
        {
            var email = _db.EmailQueue.Find(emailQueueId);
            if (email == null) return;

            email.Status = 1;     // Sent
            email.SentAt = DateTime.Now;

            _db.SaveChanges();
        }
        public void MarkAsFailed(int emailQueueId)
        {
            var email = _db.EmailQueue.Find(emailQueueId);
            if (email == null) return;

            email.Status = 2;
            email.RetryCount++;

            _db.SaveChanges();
        }

        public void ResetFailed(int emailQueueId)
        {
            var email = _db.EmailQueue.Find(emailQueueId);
            if (email == null) return;

            email.Status = 0;
            _db.SaveChanges();
        }
        public List<EmailQueue> GetAll()
        {
            return _db.EmailQueue
                      .OrderByDescending(e => e.CreatedAt)
                      .ToList();
        }
    }
}
