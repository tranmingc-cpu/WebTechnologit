using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;
using WebApplication10.Services;

namespace WebApplication10.Controllers.Admin
{
    public class AdminProductsController : BaseController
    {
        private readonly ProductDao _productDao;
        private readonly EmailQueueDAO _EmailQueueDAO;

        public AdminProductsController()
        {
            _productDao = new ProductDao(db);
            _EmailQueueDAO = new EmailQueueDAO(db);
        }

        public ActionResult Index()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            LoadAdminActions();

            var products = _productDao.GetAll()
                                      .OrderByDescending(p => p.CreatedAt)
                                      .ToList();

            return View(products);
        }

        public ActionResult Create()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Brands = db.Brands.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WebApplication10.Models.Products model)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = db.Categories.ToList();
                ViewBag.Brands = db.Brands.ToList();
                return View(model);
            }

            int productId = _productDao.Insert(model);

            EnqueueNewProductEmail(model);

            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            var product = _productDao.GetByIdAdmin(id);
            if (product == null)
                return HttpNotFound();

            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Brands = db.Brands.ToList();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WebApplication10.Models.Products model)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = db.Categories.ToList();
                ViewBag.Brands = db.Brands.ToList();
                return View(model);
            }

            _productDao.Update(model);

            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpUnauthorizedResult();

            _productDao.Delete(id);
            return Json(new { success = true });
        }

        private void EnqueueNewProductEmail(WebApplication10.Models.Products product)
        {
            var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            var productUrl = $"{baseUrl}/Products/Details/{product.ProductId}";

            var subscribers = db.NewsletterSubscribers
                                .Where(s => s.IsActive == true)
                                .ToList();

            foreach (var sub in subscribers)
            {
                _EmailQueueDAO.Enqueue(new EmailQueue
                {
                    ToEmail = sub.Email,
                    Subject = "🔥 Sản phẩm mới vừa ra mắt!",
                    Body = $@"
                <h3>{product.ProductName}</h3>
                <p>Giá: {product.Price:N0} đ</p>
                <p>{product.Description}</p>
                <a href='{productUrl}'>
                    Xem chi tiết
                </a>
            ",
                    EmailType = "NEW_PRODUCT",
                    Status = 0,
                    RetryCount = 0,
                    CreatedAt = DateTime.Now,
                    SubscriberId = sub.SubscriberId
                });
            }
        }

        public ActionResult TestSendNewProductMail(int productId)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpUnauthorizedResult();

            var product = _productDao.GetByIdAdmin(productId);
            if (product == null)
                return HttpNotFound();

            EnqueueNewProductEmail(product);

            TempData["Success"] = "✅ Đã đưa email vào queue (Pending)";
            return RedirectToAction("Index");
        }
        public ActionResult TestSendMailNow(int productId)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpUnauthorizedResult();

            var product = _productDao.GetByIdAdmin(productId);
            if (product == null)
                return HttpNotFound();

            var sender = new EmailSenderService();

            var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            var productUrl = $"{baseUrl}/Products/Details/{product.ProductId}";

            var subscribers = db.NewsletterSubscribers
                                .Where(s => s.IsActive == true)
                                .ToList();

            foreach (var sub in subscribers)
            {
                bool sent = sender.SendMail(
                    sub.Email,
                    "🔥 Sản phẩm mới vừa ra mắt!",
                    $@"
                <h3>{product.ProductName}</h3>
                <p><b>Giá:</b> {product.Price:N0} đ</p>
                <p>{product.Description}</p>
                <a href='{productUrl}'
                   style='display:inline-block;
                          padding:10px 15px;
                          background:#dc3545;
                          color:#fff;
                          text-decoration:none;
                          border-radius:5px'>
                    Xem chi tiết
                </a>
            "
                );

                if (!sent)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"❌ Gửi mail thất bại tới {sub.Email}"
                    );
                }
            }

            TempData["Success"] = "🚀 Đã gửi mail trực tiếp";
            return RedirectToAction("Index");
        }
    }
}
