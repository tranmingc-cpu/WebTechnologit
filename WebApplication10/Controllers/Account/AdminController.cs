using System.Web.Mvc;
using WebApplication10.Models;

namespace WebApplication10.Controllers
{
    public class AdminController : Controller
    {
        private TechStoreDBEntities _context = new TechStoreDBEntities();

        public ActionResult Dashboard()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            // Example stats
            ViewBag.UserCount = Users.GetUserCount(_context);
            return View();
        }
    }
}
