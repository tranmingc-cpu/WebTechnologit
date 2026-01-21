using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication10.Models;

namespace WebApplication10.DAO
{
    public class InfoPagesDAO
    {
        private readonly TechStoreDBEntities _db;

        public InfoPagesDAO(TechStoreDBEntities db)
        {
            _db = db;
        }

        public InfoPages GetById(int id)
        {
            return _db.InfoPages.FirstOrDefault(p => p.InfoPageId == id);
        }

        public InfoPages GetBySlug(string slug)
        {
            return _db.InfoPages.FirstOrDefault(p => p.Slug == slug);
        }

        public void Update(InfoPages model)
        {
            var entity = _db.InfoPages.FirstOrDefault(p => p.InfoPageId == model.InfoPageId);
            if (entity != null)
            {
                entity.Title = model.Title ?? entity.Title;
                entity.Content = model.Content ?? entity.Content;
                entity.UpdatedAt = DateTime.Now;
                _db.SaveChanges();
            }
        }
        public List<InfoPages> GetAll()
        {
            return _db.InfoPages.OrderBy(p => p.Title).ToList();
        }

        public List<InfoPages> GetAllForAdmin()
        {
            return _db.InfoPages.OrderByDescending(p => p.CreatedAt).ToList();
        }

    }
}
