using System.Linq;
using System.Web.Mvc;
using WebApplication10.Models;

namespace WebApplication10.Controllers
{
    public class AccountController : Controller
    {
        // Use the EF context
        private TechStoreDBEntities _context = new TechStoreDBEntities();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                Session["User"] = user.Email;
                Session["UserName"] = user.FullName;
                Session["UserRole"] = user.Role;

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin"); 
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullName, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View();
            }

            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email already registered";
                return View();
            }

            var newUser = new Users
            {
                Username = email,      
                FullName = fullName,
                Email = email,
                Password = password,
                Role = "Customer",
                IsActive = true,
                CreatedAt = System.DateTime.Now
            };

            try
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"{ve.PropertyName}: {ve.ErrorMessage}");
                    }
                }

                ViewBag.Error = "Unable to create account. Please check your inputs.";
                return View();
            }

            return RedirectToAction("Login");
        }


        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                // TODO: generate reset token, save to DB, send email
            }

            ViewBag.Message = "If this email exists, a reset link has been sent.";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
