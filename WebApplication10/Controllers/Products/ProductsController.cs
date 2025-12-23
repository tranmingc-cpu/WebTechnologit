using System.Linq;
using System.Web.Mvc;
using WebApplication10.Models;

namespace WebApplication10.Controllers.Products
{
    public class ProductsController : BaseController
    {
        public ActionResult Index(int? categoryId, string keyword)
        {
            var products = db.Products
                .Include("Brands")
                .Include("Categories")
                .Where(p => p.Status == "Available");

            if (!string.IsNullOrEmpty(keyword))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(keyword) ||
                    p.Brands.BrandName.Contains(keyword) ||
                    p.Categories.CategoryName.Contains(keyword)
                );

                ViewBag.Keyword = keyword;
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);

                ViewBag.CurrentCategoryId = categoryId;
                ViewBag.CurrentCategoryName = db.Categories
                    .Where(c => c.CategoryId == categoryId.Value)
                    .Select(c => c.CategoryName)
                    .FirstOrDefault();
            }

            ViewBag.TotalProducts = products.Count();

            return View(products.ToList());
        }
    }
}