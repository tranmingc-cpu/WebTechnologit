using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;

namespace WebApplication10.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminDao _adminDao;

        public AdminController()
        {
            _adminDao = new AdminDao(new TechStoreDBEntities());
        }

        public ActionResult Dashboard()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserCount = _adminDao.GetUserCount();
            return View();
        }
    }
}
