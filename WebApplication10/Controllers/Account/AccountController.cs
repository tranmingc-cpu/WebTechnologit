using System.Linq;
using System.Web.Mvc;
using WebApplication10.Models;
using UserDao = WebApplication10.DAO.UserDao;

namespace WebApplication10.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserDao _userDao;

        public AccountController()
        {
            _userDao = new UserDao(db);
        }

        // LOGIN
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(u =>
                u.Email == email &&
                u.Password == password &&
                u.IsActive == true
            );

            if (user != null)
            {
                Session["UserId"] = user.UserId;
                Session["UserEmail"] = user.Email;
                Session["UserName"] = user.FullName;
                Session["UserRole"] = user.Role;

                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng";
            return View();
        }

        // REGISTER
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

            if (_userDao.GetAll().Any(u => u.Email == email))
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
                _userDao.Add(newUser);
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

        // FORGOT PASSWORD
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            var user = _userDao.GetAll().FirstOrDefault(u => u.Email == email);

            ViewBag.Message = "If this email exists, a reset link has been sent.";
            return View();
        }

        // LOGOUT
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        // PROFILE
        [HttpGet]
        public ActionResult Profile()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];
            var user = _userDao.GetById(userId);

            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        // EDIT PROFILE
        [HttpGet]
        public ActionResult EditProfile()
        {
            int userId = (int)Session["UserId"];
            var user = _userDao.GetById(userId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(Users model)
        {
            if (ModelState.IsValid)
            {
                _userDao.Update(model);
                return RedirectToAction("Profile");
            }
            return View(model);
        }

        // CHANGE PASSWORD
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldPassword, string newPassword)
        {
            int userId = (int)Session["UserId"];
            bool success = _userDao.ChangePassword(userId, oldPassword, newPassword);

            ViewBag.Message = success ? "Đổi mật khẩu thành công!" : "Mật khẩu cũ không đúng!";
            return View();
        }
    }
}
