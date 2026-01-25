using System.Linq;
using System.Web.Mvc;
using WebApplication10.Models;
using UserDao = WebApplication10.DAO.UserDao;
using WebApplication10.Services;
using System;
using System.Collections.Generic;
using WebApplication10.ViewModels;

namespace WebApplication10.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserDao _userDao;

        public AccountController()
        {
            _userDao = new UserDao(db);
        }

        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.RightPanel = "AuthPartials/_RightPanel_Login";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                ModelState.AddModelError("email", "Email không được để trống");

            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("password", "Mật khẩu không được để trống");

            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView("AuthPartials/_LoginPartial");

                return View();
            }

            var user = db.Users.FirstOrDefault(u =>
                u.Email == email &&
                u.Password == password &&
                u.IsActive == true
            );

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");

                if (Request.IsAjaxRequest())
                    return PartialView("AuthPartials/_LoginPartial");

                return View();
            }

            Session["UserId"] = user.UserId;
            Session["UserEmail"] = user.Email;
            Session["UserName"] = user.FullName;
            Session["UserRole"] = user.Role;

            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    redirect = user.Role == "Admin"
                        ? Url.Action("Dashboard", "Admin")
                        : Url.Action("Index", "Home")
                });
            }

            return RedirectToAction(
                user.Role == "Admin" ? "Dashboard" : "Index",
                user.Role == "Admin" ? "Admin" : "Home"
            );
        }

        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.RightPanel = "AuthPartials/_RightPanel_Register";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                ModelState.AddModelError("fullName", "Họ tên không được để trống");

            if (string.IsNullOrWhiteSpace(email))
                ModelState.AddModelError("email", "Email không được để trống");

            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("password", "Mật khẩu không được để trống");

            if (password != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp");

            if (_userDao.GetAll().Any(u => u.Email == email))
                ModelState.AddModelError("email", "Email đã được đăng ký");

            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView("AuthPartials/_RegisterPartial");

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

            _userDao.Add(newUser);

            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    redirect = Url.Action("Login", "Account")
                });
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            ViewBag.RightPanel = "AuthPartials/_RightPanel_ForgotPassword";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                ModelState.AddModelError("email", "Vui lòng nhập email");

            if (!ModelState.IsValid)
                return View();

            var user = db.Users.FirstOrDefault(x =>
                x.Email == email &&
                x.IsActive == true
            );

            if (user != null)
            {
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                var service = new ResetPasswordMailService(db);
                service.QueueResetPasswordMail(email, baseUrl);

                new EmailQueueProcessor(db).Process();
            }

            ViewBag.Message = "Nếu email tồn tại, chúng tôi đã gửi liên kết đặt lại mật khẩu.";
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            var reset = db.PasswordResetTokens.FirstOrDefault(x =>
                x.Token == token &&
                x.Used == false &&
                x.ExpiredAt > DateTime.Now
            );

            if (reset == null)
                return View("ResetPasswordExpired");

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError("newPassword", "Mật khẩu không được để trống");
                ViewBag.Token = token;
                return PartialView("AuthPartials/_ResetPasswordPartial");
            }

            var reset = db.PasswordResetTokens.FirstOrDefault(x =>
                x.Token == token &&
                x.Used == false &&
                x.ExpiredAt > DateTime.Now
            );

            if (reset == null)
                return View("ResetPasswordExpired");

            var user = db.Users.FirstOrDefault(x => x.Email == reset.Email);
            if (user == null)
                return View("ResetPasswordExpired");

            user.Password = newPassword;
            reset.Used = true;

            db.SaveChanges();

            TempData["Message"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

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

            bool isSubscribed = db.NewsletterSubscribers
                .Any(x => x.Email == user.Email && x.IsActive == true);

            ViewBag.IsNewsletterSubscribed = isSubscribed;

            return View(user);
        }

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
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldPassword, string newPassword)
        {
            int userId = (int)Session["UserId"];
            bool success = _userDao.ChangePassword(userId, oldPassword, newPassword);
            return Json(new { success = success, message = success ? "Đổi mật khẩu thành công!" : "Mật khẩu cũ không đúng!" });
        }

        protected bool IsAjax()
        {
            return Request.IsAjaxRequest();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        // ORDERS - Lịch sử đơn hàng
        [HttpGet]
        public ActionResult Orders()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];
            
            var orders = db.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate ?? DateTime.Now,
                    TotalAmount = o.TotalAmount ?? 0,
                    Status = o.Status,
                    ShippingAddress = o.ShippingAddress,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailViewModel
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Products.ProductName,
                        ImageUrl = od.Products.ImageUrl,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        TotalPrice = od.Quantity * od.UnitPrice
                    }).ToList()
                })
                .ToList();

            return View(orders);
        }

        // Chi tiết đơn hàng
        [HttpGet]
        public ActionResult OrderDetail(int id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];
            
            var order = db.Orders
                .Where(o => o.OrderId == id && o.UserId == userId)
                .Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate ?? DateTime.Now,
                    TotalAmount = o.TotalAmount ?? 0,
                    Status = o.Status,
                    ShippingAddress = o.ShippingAddress,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailViewModel
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Products.ProductName,
                        ImageUrl = od.Products.ImageUrl,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        TotalPrice = od.Quantity * od.UnitPrice
                    }).ToList()
                })
                .FirstOrDefault();

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // Hủy đơn hàng
        [HttpPost]
        public ActionResult CancelOrder(int orderId)
        {
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            int userId = (int)Session["UserId"];
            
            var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);
            
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            if (order.Status != "Pending")
            {
                return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xử lý" });
            }

            order.Status = "Cancelled";
            
            // Hoàn lại số lượng sản phẩm
            foreach (var detail in order.OrderDetails)
            {
                var product = db.Products.Find(detail.ProductId);
                if (product != null)
                {
                    product.Quantity = (product.Quantity ?? 0) + detail.Quantity;
                }
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Đã hủy đơn hàng thành công" });
        }
    }
}
