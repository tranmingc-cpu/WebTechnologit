using System.Linq;
using System.Web.Mvc;
using WebApplication10.Models;
using System.Data.Entity;

namespace WebApplication10.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories
                                   .OrderBy(c => c.CategoryName)
                                   .Take(6)
                                   .ToList();

            var featuredProducts = db.Products
                .Include(p => p.Brands)
                .Include(p => p.Categories)
                .Where(p => p.Status == "Available")
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
    }
}
