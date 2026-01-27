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
            ViewBag.RightPanel = "AuthPartials/_LoginRight";
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
            ViewBag.RightPanel = "AuthPartials/_RegisterRight";
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
        [HttpGet]
        public ActionResult UserProfile()
        {
            if (Session["UserId"] == null)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { redirect = Url.Action("Login", "Account") }, JsonRequestBehavior.AllowGet);

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

            if (Request.IsAjaxRequest())
                return PartialView("Profile/_ProfileContent", user);

            return View("Profile/_ProfileContent", user);
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
        public ActionResult ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            k => k.Key,
                            v => v.Value.Errors.First().ErrorMessage
                        )
                });
            }

            int userId = (int)Session["UserId"];

            bool success = _userDao.ChangePassword(
                userId,
                model.OldPassword,
                model.NewPassword
            );

            if (!success)
            {
                return Json(new
                {
                    success = false,
                    message = "Mật khẩu cũ không đúng"
                });
            }

            return Json(new
            {
                success = true,
                message = "Đổi mật khẩu thành công!"
            });
        }

        protected bool IsAjax()
        {
            return Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        // ORDERS - Lịch sử đơn hàng
        [HttpGet]
        public ActionResult Orders()
        {
            var orders = new List<OrderViewModel>();

            try
            {
                System.Diagnostics.Debug.WriteLine("=== Orders Action Started ===");
                
                if (Session["UserId"] != null)
                {
                    // User đã đăng nhập - lấy tất cả đơn hàng
                    int userId = (int)Session["UserId"];
                    System.Diagnostics.Debug.WriteLine($"Logged-in user: {userId}");
                    
                    var userOrders = db.Orders
                        .Where(o => o.UserId == userId)
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"Found {userOrders.Count} orders for user {userId}");

                    foreach (var o in userOrders)
                    {
                        try
                        {
                            var orderDetails = o.OrderDetails.Select(od => new OrderDetailViewModel
                            {
                                ProductId = od.ProductId,
                                ProductName = od.Products != null ? od.Products.ProductName : "N/A",
                                ImageUrl = od.Products != null ? od.Products.ImageUrl : "",
                                Quantity = od.Quantity,
                                UnitPrice = od.UnitPrice,
                                TotalPrice = od.Quantity * od.UnitPrice
                            }).ToList();

                            orders.Add(new OrderViewModel
                            {
                                OrderId = o.OrderId,
                                OrderDate = o.OrderDate ?? DateTime.Now,
                                TotalAmount = o.TotalAmount ?? 0,
                                Status = o.Status,
                                ShippingAddress = o.ShippingAddress,
                                OrderDetails = orderDetails
                            });
                        }
                        catch (Exception orderEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing order {o.OrderId}: {orderEx.Message}");
                            // Skip this order and continue
                        }
                    }
                }
                else if (Session["GuestOrders"] != null)
                {
                    // Guest user - lấy đơn hàng từ session
                    var guestOrderIds = Session["GuestOrders"] as List<int>;
                    System.Diagnostics.Debug.WriteLine($"Guest orders: {string.Join(", ", guestOrderIds ?? new List<int>())}");
                    
                    if (guestOrderIds != null && guestOrderIds.Any())
                    {
                        var guestOrders = db.Orders
                            .Where(o => guestOrderIds.Contains(o.OrderId))
                            .OrderByDescending(o => o.OrderDate)
                            .ToList();

                        System.Diagnostics.Debug.WriteLine($"Found {guestOrders.Count} guest orders");

                        foreach (var o in guestOrders)
                        {
                            try
                            {
                                var orderDetails = o.OrderDetails.Select(od => new OrderDetailViewModel
                                {
                                    ProductId = od.ProductId,
                                    ProductName = od.Products != null ? od.Products.ProductName : "N/A",
                                    ImageUrl = od.Products != null ? od.Products.ImageUrl : "",
                                    Quantity = od.Quantity,
                                    UnitPrice = od.UnitPrice,
                                    TotalPrice = od.Quantity * od.UnitPrice
                                }).ToList();

                                orders.Add(new OrderViewModel
                                {
                                    OrderId = o.OrderId,
                                    OrderDate = o.OrderDate ?? DateTime.Now,
                                    TotalAmount = o.TotalAmount ?? 0,
                                    Status = o.Status,
                                    ShippingAddress = o.ShippingAddress,
                                    OrderDetails = orderDetails
                                });
                            }
                            catch (Exception orderEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error processing guest order {o.OrderId}: {orderEx.Message}");
                                // Skip this order and continue
                            }
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No user session and no guest orders");
                }

                System.Diagnostics.Debug.WriteLine($"Total orders to display: {orders.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Orders Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }

            return View(orders);
        }

        // Chi tiết đơn hàng
        [HttpGet]
        public ActionResult OrderDetail(int id)
        {
            System.Diagnostics.Debug.WriteLine($"=== OrderDetail called with id: {id} ===");
            
            try
            {
                // Check quyền xem đơn hàng
                bool canView = false;
                
                if (Session["UserId"] != null)
                {
                    int userId = (int)Session["UserId"];
                    var checkOrder = db.Orders.FirstOrDefault(o => o.OrderId == id && o.UserId == userId);
                    canView = (checkOrder != null);
                }
                else if (Session["GuestOrders"] != null)
                {
                    var guestOrderIds = Session["GuestOrders"] as List<int>;
                    canView = (guestOrderIds != null && guestOrderIds.Contains(id));
                }

                if (!canView)
                {
                    if (IsAjax())
                    {
                        return Json(new { success = false, message = "Bạn không có quyền xem đơn hàng này" }, JsonRequestBehavior.AllowGet);
                    }
                    return RedirectToAction("Login", "Account");
                }

                // Load order
                var orderEntity = db.Orders.Find(id);

                if (orderEntity == null)
                {
                    if (IsAjax())
                    {
                        return Json(new { success = false, message = "Không tìm thấy đơn hàng" }, JsonRequestBehavior.AllowGet);
                    }
                    return HttpNotFound();
                }

                // Map to ViewModel
                var orderDetailsList = new List<OrderDetailViewModel>();
                
                if (orderEntity.OrderDetails != null)
                {
                    foreach (var od in orderEntity.OrderDetails)
                    {
                        orderDetailsList.Add(new OrderDetailViewModel
                        {
                            ProductId = od.ProductId,
                            ProductName = od.Products?.ProductName ?? "Sản phẩm không tồn tại",
                            ImageUrl = od.Products?.ImageUrl ?? "",
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice,
                            TotalPrice = od.Quantity * od.UnitPrice
                        });
                    }
                }

                var orderViewModel = new OrderViewModel
                {
                    OrderId = orderEntity.OrderId,
                    OrderDate = orderEntity.OrderDate ?? DateTime.Now,
                    TotalAmount = orderEntity.TotalAmount ?? 0,
                    Status = orderEntity.Status ?? "Pending",
                    ShippingAddress = orderEntity.ShippingAddress ?? "",
                    OrderDetails = orderDetailsList
                };

                // Return JSON for AJAX request
                if (IsAjax())
                {
                    return Json(new 
                    { 
                        success = true,
                        order = new
                        {
                            orderId = orderViewModel.OrderId,
                            orderDate = orderViewModel.OrderDate.ToString("dd/MM/yyyy HH:mm"),
                            totalAmount = orderViewModel.TotalAmount,
                            status = orderViewModel.Status,
                            shippingAddress = orderViewModel.ShippingAddress,
                            orderDetails = orderViewModel.OrderDetails.Select(od => new
                            {
                                productId = od.ProductId,
                                productName = od.ProductName,
                                imageUrl = string.IsNullOrEmpty(od.ImageUrl) ? "/Content/images/no-image.png" : "/Content/images/" + od.ImageUrl,
                                quantity = od.Quantity,
                                unitPrice = od.UnitPrice,
                                totalPrice = od.TotalPrice
                            }).ToList()
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                return View(orderViewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in OrderDetail: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (IsAjax())
                {
                    return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
                
                return Content($"Error: {ex.Message}");
            }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfileAjax(Users model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Profile/_EditProfileModal", model);
            }

            _userDao.Update(model);

            return Json(new { success = true });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePasswordAjax(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(
                    "Profile/_ChangePasswordModal",
                    model
                );
            }

            int userId = (int)Session["UserId"];

            bool success = _userDao.ChangePassword(
                userId,
                model.OldPassword,
                model.NewPassword
            );

            if (!success)
            {
                ModelState.AddModelError(
                    "OldPassword",
                    "Mật khẩu cũ không đúng"
                );

                return PartialView("Profile/_ChangePasswordModal", model);
            }
            return Json(new { success = true });
        }

    }
}
