using System;
using System.Linq;
using WebApplication10.Models;

namespace WebApplication10.DAO
{
    public class AboutDAO
    {
        private readonly TechStoreDBEntities _context;

        public AboutDAO(TechStoreDBEntities context)
        {
            _context = context;
        }

        public AboutPage GetAbout()
        {
            var about = _context.AboutPage.FirstOrDefault();

            if (about == null)
            {
                about = new AboutPage
                {
                    Title = "Giới thiệu",
                    Description = "",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.AboutPage.Add(about);
                _context.SaveChanges();
            }

            return about;
        }

        public void Update(AboutPage model)
        {
            var about = _context.AboutPage.FirstOrDefault();

            if (about == null)
            {
                about = new AboutPage
                {
                    CreatedAt = DateTime.Now
                };
                _context.AboutPage.Add(about);
            }

            about.Title = model.Title;
            about.Description = model.Description;
            about.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
        }
    }
}
