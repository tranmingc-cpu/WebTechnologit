using System;
using System.Net;
using System.Web.Mvc;
using WebApplication10.Models;
using WebApplication10.DAO;
using WebApplication10.ViewModels;

namespace WebApplication10.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly ProductDao _productDao;
        private readonly CategoryDAO _categoryDao;
        private readonly BrandDao _brandDao;

        public ProductsController()
        {
            var db = new TechStoreDBEntities();
            _productDao = new ProductDao(db);
            _categoryDao = new CategoryDAO(db);
            _brandDao = new BrandDao(db);
        }

        public ActionResult Index()
        {
            var data = _productDao.GetAllWithCategoryBrand();
            return PartialView("_ProductsPartial", data);
        }

        public ActionResult ProductListPartial()
        {
            var products = _productDao.GetAllWithCategoryBrand();
            return PartialView("_ProductsPartial", products);
        }

        // ================= CREATE =================
        [HttpGet]
        public ActionResult Create()
        {
            var vm = new AdminCreateProductVM
            {
                Categories = _categoryDao.GetAll(),
                Brands = _brandDao.GetAll()
            };
            return PartialView("_CreateProduct", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminCreateProductVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _categoryDao.GetAll();
                model.Brands = _brandDao.GetAll();
                return PartialView("_CreateProduct", model);
            }

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
                Status = model.Status,
                CreatedAt = DateTime.Now
            };

            _productDao.Add(product);
            return Json(new { success = true, message = "Thêm sản phẩm thành công!" });
        }

        // ================= EDIT =================
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var product = _productDao.GetById(id);
            if (product == null) return HttpNotFound();

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
                Status = product.Status,
                Categories = _categoryDao.GetAll(),
                Brands = _brandDao.GetAll()
            };

            return PartialView("_EditProduct", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminCreateProductVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _categoryDao.GetAll();
                model.Brands = _brandDao.GetAll();
                return PartialView("_EditProduct", model);
            }

            var product = _productDao.GetById(model.ProductId);
            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });

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

        // ================= DELETE =================
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var product = _productDao.GetById(id);
            if (product == null) return HttpNotFound();
            return PartialView("_DeleteProduct", product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int productId)
        {
            if (_productDao.Delete(productId, out string message))
            {
                return Json(new
                {
                    success = true,
                    message = message == "" ? "Xóa sản phẩm thành công!" : message
                });
            }

            return Json(new
            {
                success = false,
                message = message
            });
        }

    }
}
