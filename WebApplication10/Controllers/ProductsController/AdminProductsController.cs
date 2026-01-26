using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;
using WebApplication10.Services;
using WebApplication10.ViewModels;


namespace WebApplication10.Controllers.Admin
{
    public class AdminProductsController : BaseController
    {
        private readonly ProductDao _productDao;
        private readonly EmailQueueDAO _EmailQueueDAO;
        private readonly CategoryDAO _categoryDao;
        private readonly BrandDao _brandDao;

        public AdminProductsController()
        {
            _productDao = new ProductDao(db);
            _EmailQueueDAO = new EmailQueueDAO(db);
            _categoryDao = new CategoryDAO(db);
            _brandDao = new BrandDao(db);
        }

        public ActionResult Index()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            LoadAdminActions();

            return View();
        }

        private void LoadCategoryBrand()
        {
            ViewBag.Categories = _categoryDao.GetAll();
            ViewBag.Brands = _brandDao.GetAll();
        }

        public ActionResult ProductListPartial()
        {
            var products = _productDao.GetAll()
                .Select(p => new ProductListVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,

                    CategoryName = p.Categories != null
                        ? p.Categories.CategoryName
                        : "",

                    BrandName = p.Brands != null
                        ? p.Brands.BrandName
                        : "",

                    Price = p.Price,
                    Quantity = p.Quantity,
                    Status = p.Status
                })
                .ToList();

            return PartialView("_ProductsPartial", products);
        }

        // ================= CREATE =================
        [HttpGet]
        public ActionResult Create()
        {
            LoadCategoryBrand();
            return PartialView("_CreateProduct", new AdminCreateProductVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminCreateProductVM model)
        {
            if (!ModelState.IsValid)
            {
                LoadCategoryBrand(); 
                return PartialView("_CreateProduct", model);
            }
            try
            {
                var product = new Models.Products
                {
                    ProductName = model.ProductName,
                    CategoryId = model.CategoryId,
                    BrandId = model.BrandId,
                    Price = model.Price,
                    Discount = model.Discount,
                    Quantity = model.Quantity,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    Status = model.Status ?? "Available",
                    CreatedAt = DateTime.Now
                };

                _productDao.Add(product);

                return Json(new { success = true, message = "Thêm sản phẩm thành công!" });
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm sản phẩm.");
                return PartialView("_CreateProduct", model);
            }
        }

        // ================= EDIT =================
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var product = _productDao.GetById(id.Value);
            if (product == null)
                return HttpNotFound();
            LoadCategoryBrand();

            var vm = new AdminEditProductsVM
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                Price = product.Price,
                Discount = product.Discount,
                Quantity = product.Quantity,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Status = product.Status
            };

            return PartialView("_EditProducts", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminEditProductsVM model)
        {
            if (!ModelState.IsValid) { 
                LoadCategoryBrand();
            return PartialView("_EditProducts", model);
        }
            var product = _productDao.GetById(model.ProductId);
            if (product == null)
            {
                LoadCategoryBrand();

                ModelState.AddModelError("", "Sản phẩm không tồn tại.");
                return PartialView("_EditProducts", model);
            }

            try
            {
                product.ProductName = model.ProductName;
                product.CategoryId = model.CategoryId;
                product.BrandId = model.BrandId;
                product.Price = model.Price;
                product.Discount = model.Discount;
                product.Quantity = model.Quantity;
                product.Description = model.Description;
                product.ImageUrl = model.ImageUrl;
                product.Status = model.Status;

                _productDao.Update(product);

                return Json(new { success = true, message = "Cập nhật sản phẩm thành công!" });
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật sản phẩm.");
                return PartialView("_EditProduct", model);
            }
        }

        // ================= DELETE =================
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var product = _productDao.GetById(id.Value);
            if (product == null)
                return HttpNotFound();

            return PartialView("_DeleteProduct", product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int productId)
        {
            if (_productDao.Delete(productId, out string message))
            {
                return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
            }

            return Json(new { success = false, message });
        }
        [HttpGet]
        public ActionResult ProductDetails(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = _productDao.GetProductDetails(id.Value);
            if (product == null)
                return HttpNotFound();

            return PartialView("_ProductDetails", product);
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