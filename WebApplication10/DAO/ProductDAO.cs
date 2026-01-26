using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication10.Models;
using System.Data.Entity;
using WebApplication10.ViewModels;


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
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int productId, out string message)
        {
            message = "";

            try
            {
                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    message = "Sản phẩm không tồn tại.";
                    return false;
                }

                product.Status = "Unavailable";
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                message = "Không thể xóa sản phẩm. Sản phẩm có thể đang được sử dụng.";
                return false;
            }
        }

        public IQueryable<Products> GetAll()
        {
            return _context.Products;
                           
        }
        public Products GetByIdAdmin(int id)
        {
            return _context.Products
                           .Include(p => p.Brands)
                           .Include(p => p.Categories)
                           .FirstOrDefault(p => p.ProductId == id);
        }
        public List<ProductListVM> GetAllWithCategoryBrand()
        {
            return _context.Products
                .Select(p => new ProductListVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,

                    CategoryId = p.CategoryId,
                    CategoryName = p.Categories.CategoryName,

                    BrandId = p.BrandId,
                    BrandName = p.Brands != null ? p.Brands.BrandName : "",

                    Price = p.Price,
                    Discount = p.Discount,
                    Quantity = p.Quantity,

                    Status = p.Status
                })
                .OrderByDescending(p => p.ProductId)
                .ToList();
        }
        public void Add(Products product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }
        public ProductListVM GetProductDetails(int productId)
        {
            return _context.Products
       .Where(p => p.ProductId == productId)
       .Select(p => new ProductListVM
       {
           ProductId = p.ProductId,
           ProductName = p.ProductName,
           Price = p.Price,
           Discount = p.Discount,
           Quantity = p.Quantity,
           Description = p.Description,
           ImageUrl = p.ImageUrl,
           Status = p.Status,
           CreatedAt = p.CreatedAt,

           CategoryName = p.Categories.CategoryName,
           BrandName = p.Brands != null ? p.Brands.BrandName : ""
       })
       .FirstOrDefault();
        }

    }
}
