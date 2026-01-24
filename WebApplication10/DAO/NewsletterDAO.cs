using System;
using System.Linq;
using WebApplication10.Models;

namespace WebApplication10.DAO
{
    public class NewsletterDAO
    {
        private readonly TechStoreDBEntities _context;

        public NewsletterDAO(TechStoreDBEntities context)
        {
            _context = context;
        }

        public bool Exists(string email)
        {
            return _context.NewsletterSubscribers.Any(x =>
                x.Email == email &&
                (x.IsActive == true || x.IsActive == null)
            );
        }
        public void Add(string email, int? userId, string source)
        {
            var existing = _context.NewsletterSubscribers
                .FirstOrDefault(x => x.Email == email);

            if (existing != null)
            {
                existing.IsActive = true;
                existing.UnsubscribedAt = null;
                existing.UserId = userId;
                existing.Source = source;
            }
            else
            {
                _context.NewsletterSubscribers.Add(new NewsletterSubscribers
                {
                    Email = email,
                    UserId = userId,
                    Source = source,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                });
            }

            _context.SaveChanges();
        }

        public bool Unsubscribe(string email)
        {
            var subscriber = _context.NewsletterSubscribers
                .FirstOrDefault(x => x.Email == email && x.IsActive == true);

            if (subscriber == null)
                return false;

            subscriber.IsActive = false;
            subscriber.UnsubscribedAt = DateTime.Now;

            _context.SaveChanges();
            return true;
        }

        public bool UnsubscribeByEmail(string email)
        {
            var subscriber = _context.NewsletterSubscribers
                .FirstOrDefault(x => x.Email == email && x.IsActive == true);

            if (subscriber == null)
                return false;

            subscriber.IsActive = false;
            subscriber.UnsubscribedAt = DateTime.Now;
            _context.SaveChanges();

            return true;
        }

        public IQueryable<NewsletterSubscribers> GetActiveSubscribers()
        {
            return _context.NewsletterSubscribers
                .Where(x => x.IsActive == true);
        }
        public bool IsSubscribed(int userId)
        {
            return _context.NewsletterSubscribers
                .Any(x => x.UserId == userId && x.IsActive == true);
        }
    }
}
