using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication10.Models;

namespace WebApplication10.DAO
{
    public class CategoryDAO
    {
        private readonly TechStoreDBEntities _db;

        public CategoryDAO(TechStoreDBEntities db)
        {
            _db = db;
        }

        public Categories GetById(int id)
        {
            return _db.Categories
                      .FirstOrDefault(c => c.CategoryId == id);
        }

        public List<Categories> GetAll()
        {
            return _db.Categories
                      .OrderBy(c => c.CategoryName)
                      .ToList();
        }

        public void UpdateDescription(int categoryId, string description)
        {
            var category = _db.Categories.Find(categoryId);

            if (category == null)
                throw new Exception("Category không tồn tại");

            category.Description = description;
            _db.SaveChanges();
        }

        public void Update(Categories model)
        {
            var category = _db.Categories.Find(model.CategoryId);

            if (category == null)
                throw new Exception("Category không tồn tại");

            category.CategoryName = model.CategoryName;
            category.Description = model.Description;

            _db.SaveChanges();
        }
    }
}
