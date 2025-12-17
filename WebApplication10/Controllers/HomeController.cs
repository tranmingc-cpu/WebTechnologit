using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication10.Models;
using System.Data.Entity;

namespace WebApplication10.Controllers
{
    public class HomeController : Controller
    {
        private TechStoreDBEntities db = new TechStoreDBEntities();

        public ActionResult Index()
        {
            var products = db.Products
                .Include(p => p.Brands)
                .Include(p => p.Categories)
                .Where(p => p.Status != null && p.Status.Trim() == "Available")
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToList();

            ViewBag.ProductCount = products.Count;
            ViewBag.UserCount = db.Users.Count();

            return View(products);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            
            return View();
        }
    }
}