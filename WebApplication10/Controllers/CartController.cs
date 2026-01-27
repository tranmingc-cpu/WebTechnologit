using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication10.Models;
using WebApplication10.Services;
using WebApplication10.ViewModels;

namespace WebApplication10.Controllers
{
    public class CartController : BaseController
    {
        private const string CartSessionKey = "ShoppingCart";
        private readonly EmailService _emailService;

        public CartController()
        {
            _emailService = new EmailService();
        }

        private List<CartItem> GetCartFromSession()
        {
            var cart = Session[CartSessionKey] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session[CartSessionKey] = cart;
            }
            return cart;
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            Session[CartSessionKey] = cart;
        }

        public ActionResult Index()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = db.Products.Find(productId);
            if (product == null || product.Status != "Available")
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc không khả dụng" }, JsonRequestBehavior.AllowGet);
            }

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + quantity;
                if (newQuantity > product.Quantity)
                {
                    return Json(new { success = false, message = "Số lượng vượt quá tồn kho" }, JsonRequestBehavior.AllowGet);
                }
                existingItem.Quantity = newQuantity;
            }
            else
            {
                if (quantity > product.Quantity)
                {
                    return Json(new { success = false, message = "Số lượng vượt quá tồn kho" }, JsonRequestBehavior.AllowGet);
                }

                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Discount = product.Discount,
                    Quantity = quantity,
                    Stock = product.Quantity ?? 0
                });
            }

            SaveCartToSession(cart);

            return Json(new 
            { 
                success = true, 
                message = "Đã thêm vào giỏ hàng",
                cartCount = cart.Sum(c => c.Quantity)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity < 1)
            {
                return Json(new { success = false, message = "Số lượng phải lớn hơn 0" }, JsonRequestBehavior.AllowGet);
            }

            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng" }, JsonRequestBehavior.AllowGet);
            }

            var product = db.Products.Find(productId);
            if (quantity > product.Quantity)
            {
                return Json(new { success = false, message = "Số lượng vượt quá tồn kho" }, JsonRequestBehavior.AllowGet);
            }

            item.Quantity = quantity;
            SaveCartToSession(cart);

            return Json(new 
            { 
                success = true,
                itemTotal = item.TotalPrice,
                cartTotal = cart.Sum(c => c.TotalPrice),
                cartCount = cart.Sum(c => c.Quantity)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCartToSession(cart);
            }

            return Json(new 
            { 
                success = true,
                cartTotal = cart.Sum(c => c.TotalPrice),
                cartCount = cart.Sum(c => c.Quantity)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ClearCart()
        {
            Session[CartSessionKey] = new List<CartItem>();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCartCount()
        {
            var cart = GetCartFromSession();
            return Json(new { count = cart.Sum(c => c.Quantity) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Checkout()
        {
            var cart = GetCartFromSession();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index");
            }
            return View(cart);
        }

        [HttpPost]
        public ActionResult ProcessCheckout(string fullName, string phone, string email, 
                                           string address, string notes, string paymentMethod)
        {
            var cart = GetCartFromSession();
            if (cart == null || !cart.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Lấy UserId nếu đã đăng nhập, không thì để null
                int? userId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;

                // Tạo đơn hàng mới
                var order = new Orders
                {
                    UserId = userId ?? 0,
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.Sum(c => c.TotalPrice),
                    Status = "Paid",
                    ShippingAddress = address
                };

                db.Orders.Add(order);
                db.SaveChanges();

                // Lưu order ID vào session cho guest user
                if (userId == null || userId == 0)
                {
                    var guestOrders = Session["GuestOrders"] as List<int> ?? new List<int>();
                    guestOrders.Add(order.OrderId);
                    Session["GuestOrders"] = guestOrders;
                    
                    // Lưu thông tin khách
                    Session["GuestName"] = fullName;
                    Session["GuestEmail"] = email;
                    Session["GuestPhone"] = phone;
                }

                // Tạo chi tiết đơn hàng
                var orderItems = new List<OrderEmailItemViewModel>();
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetails
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    db.OrderDetails.Add(orderDetail);

                    // Cập nhật số lượng tồn kho
                    var product = db.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity = (product.Quantity ?? 0) - item.Quantity;
                    }

                    // Thêm vào danh sách để gửi email
                    orderItems.Add(new OrderEmailItemViewModel
                    {
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    });
                }

                db.SaveChanges();

                // Gửi email xác nhận đơn hàng
                try
                {
                    var orderEmailInfo = new OrderEmailViewModel
                    {
                        OrderId = order.OrderId,
                        OrderDate = order.OrderDate ?? DateTime.Now,
                        CustomerName = fullName,
                        CustomerEmail = email,
                        CustomerPhone = phone,
                        ShippingAddress = address,
                        TotalAmount = order.TotalAmount ?? 0,
                        OrderItems = orderItems
                    };

                    // Gửi email (không chặn luồng chính nếu email fail)
                    _emailService.SendOrderConfirmationEmail(orderEmailInfo);
                }
                catch (Exception emailEx)
                {
                    // Log email error nhưng không ảnh hưởng đến đơn hàng
                    System.Diagnostics.Debug.WriteLine($"Email sending failed: {emailEx.Message}");
                }

                // Xóa giỏ hàng
                Session[CartSessionKey] = new List<CartItem>();
                
                // Trả về JSON response cho AJAX
                return Json(new 
                { 
                    success = true, 
                    message = "Cảm ơn bạn đã đặt hàng! Chúng tôi đã gửi email xác nhận đến " + email,
                    orderId = order.OrderId
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Checkout Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }
    }
}
