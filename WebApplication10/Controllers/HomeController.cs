using System.Linq;
using System.Web.Mvc;
using WebApplication10.Models;
using WebApplication10.DAO;

namespace WebApplication10.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ProductDao _productDao;

        public HomeController()
        {
            _productDao = new ProductDao(db);
        }

        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories
                                   .OrderBy(c => c.CategoryName)
                                   .Take(6)
                                   .ToList();

            var featuredProducts = _productDao.GetAvailableProducts()
                                              .OrderByDescending(p => p.CreatedAt)
                                              .Take(8)
                                              .ToList();

            return View(featuredProducts);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult LoadProductsByCategory(int? categoryId)
        {
            var products = _productDao.GetAvailableProducts();

            if (categoryId.HasValue)
            {
                products = _productDao.GetByCategory(categoryId.Value);
            }

            var result = products.OrderByDescending(p => p.CreatedAt)
                                 .Take(8)
                                 .ToList();

            return PartialView("_HomeProductGrid", result);
        }
    }
}