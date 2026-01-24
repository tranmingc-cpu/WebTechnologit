using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication10.Models;
using System.Data.Entity;

namespace WebApplication10.DAO
{
    public class ProductDao
    {
        private readonly TechStoreDBEntities _context;

        public ProductDao(TechStoreDBEntities context)
        {
            _context = context;
        }

        public IQueryable<Products> GetAvailableProducts()
        {
            return _context.Products
                           .Include(p => p.Brands)
                           .Include(p => p.Categories)
                           .Where(p => p.Status == "Available");
        }

        public IQueryable<Products> GetByCategory(int categoryId)
        {
            return GetAvailableProducts()
                   .Where(p => p.CategoryId == categoryId);
        }

        public IQueryable<Products> Search(string keyword)
        {
            var query = GetAvailableProducts();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p =>
                    p.ProductName.Contains(keyword) ||
                    p.Brands.BrandName.Contains(keyword) ||
                    p.Categories.CategoryName.Contains(keyword)
                );
            }
            return query;
        }

        public Products GetById(int id)
        {
            return _context.Products
                           .Include(p => p.Brands)
                           .Include(p => p.Categories)
                           .FirstOrDefault(p => p.ProductId == id);
        }

        public List<Products> GetLatest(int? categoryId = null, int top = 0)
        {
            var query = GetAvailableProducts();
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            query = query.OrderByDescending(p => p.CreatedAt);

            if (top > 0)
                return query.Take(top).ToList();

            return query.ToList();
        }

        public Categories GetCategory(int categoryId)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
        }

        public int Count(IQueryable<Products> products)
        {
            return products.Count();
        }

        public int Insert(Products product)
        {
            product.CreatedAt = DateTime.Now;
            product.Status = product.Status ?? "Available";

            _context.Products.Add(product);
            _context.SaveChanges();

            return product.ProductId;
        }

        public bool Update(Products product)
        {
            var existing = _context.Products.Find(product.ProductId);
            if (existing == null)
                return false;

            existing.ProductName = product.ProductName;
            existing.CategoryId = product.CategoryId;
            existing.BrandId = product.BrandId;
            existing.Price = product.Price;
            existing.Discount = product.Discount;
            existing.Quantity = product.Quantity;
            existing.Description = product.Description;
            existing.ImageUrl = product.ImageUrl;
            existing.Status = product.Status;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
                return false;

            product.Status = "Deleted";
            _context.SaveChanges();
            return true;
        }
        public IQueryable<Products> GetAll()
        {
            return _context.Products
                           .Include(p => p.Brands)
                           .Include(p => p.Categories);
        }
        public Products GetByIdAdmin(int id)
        {
            return _context.Products
                           .Include(p => p.Brands)
                           .Include(p => p.Categories)
                           .FirstOrDefault(p => p.ProductId == id);
        }

    }
}
