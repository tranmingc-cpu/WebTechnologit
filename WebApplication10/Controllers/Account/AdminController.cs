using System.Linq;
using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;
using WebApplication10.ViewModels;

namespace WebApplication10.Controllers
{
    public class AdminController : BaseController
    {
        private readonly AdminDao _adminDao;

        public AdminController()
        {
            db = new TechStoreDBEntities();
            _adminDao = new AdminDao(db);
        }

        public ActionResult Dashboard(bool partial = false)
        {
            ViewBag.IsPartial = partial;

            if (Session["UserRole"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserCount = _adminDao.GetUserCount();
            ViewBag.OrderCount = _adminDao.GetOrderCount();

            var about = db.InfoPages.FirstOrDefault(p => p.Slug == "about");
            var contact = db.InfoPages.FirstOrDefault(p => p.Slug == "contact");
            var warranty = db.InfoPages.FirstOrDefault(p => p.Slug == "warranty");
            var news = db.InfoPages.FirstOrDefault(p => p.Slug == "news");
            var careers = db.InfoPages.FirstOrDefault(p => p.Slug == "careers");
            var returns = db.InfoPages.FirstOrDefault(p => p.Slug == "returns");
            var shipping = db.InfoPages.FirstOrDefault(p => p.Slug == "shipping");
            var payment = db.InfoPages.FirstOrDefault(p => p.Slug == "payment");

            var otherPages = db.InfoPages
                .Where(p => !new[] { "about", "contact", "warranty", "news", "careers", "returns", "shipping", "payment" }.Contains(p.Slug))
                .ToList();

            var model = new AdminActionsViewModel
            {
                About = about,
                Contact = contact,
                Warranty = warranty,
                News = news,
                Careers = careers,
                Returns = returns,
                Shipping = shipping,
                Payment = payment,
                OtherPages = otherPages
            };

            if (partial)
                return PartialView(model);

            return View(model);
        }

        public ActionResult AdminActionsPartial()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpUnauthorizedResult();

            var about = db.InfoPages.FirstOrDefault(p => p.Slug == "about");
            var contact = db.InfoPages.FirstOrDefault(p => p.Slug == "contact");
            var warranty = db.InfoPages.FirstOrDefault(p => p.Slug == "warranty");
            var news = db.InfoPages.FirstOrDefault(p => p.Slug == "news");
            var careers = db.InfoPages.FirstOrDefault(p => p.Slug == "careers");
            var returns = db.InfoPages.FirstOrDefault(p => p.Slug == "returns");
            var shipping = db.InfoPages.FirstOrDefault(p => p.Slug == "shipping");
            var payment = db.InfoPages.FirstOrDefault(p => p.Slug == "payment");

            var otherPages = db.InfoPages
                .Where(p => !new[] { "about", "contact", "warranty", "news", "careers", "returns", "shipping", "payment" }.Contains(p.Slug))
                .ToList();

            var model = new AdminActionsViewModel
            {
                About = about,
                Contact = contact,
                Warranty = warranty,
                News = news,
                Careers = careers,
                Returns = returns,
                Shipping = shipping,
                Payment = payment,
                OtherPages = otherPages
            };

            return PartialView("_AdminActions", model);
        }
    }
}
