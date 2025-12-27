using System.Linq;
using System.Web.Mvc;
using WebApplication10.Models;
using WebApplication10.DAO;

namespace WebApplication10.Controllers.Products
{
    public class ProductsController : BaseController
    {
        private readonly ProductDao _productDao;

        public ProductsController()
        {
            _productDao = new ProductDao(db);
        }

        public ActionResult Index(int? categoryId, string keyword)
        {
            var products = _productDao.GetAvailableProducts();

            if (!string.IsNullOrEmpty(keyword))
            {
                products = _productDao.Search(keyword);
                ViewBag.Keyword = keyword;
            }

            if (categoryId.HasValue)
            {
                products = _productDao.GetByCategory(categoryId.Value);

                var category = _productDao.GetCategory(categoryId.Value);

                if (category != null)
                {
                    ViewBag.CurrentCategoryId = category.CategoryId;
                    ViewBag.CurrentCategoryName = category.CategoryName;
                    ViewBag.CurrentCategoryDescription = category.Description;
                }
            }

            ViewBag.TotalProducts = products.Count();

            return View(products.ToList());
        }

        public ActionResult LoadByCategory(int? categoryId)
        {
            var products = _productDao.GetAvailableProducts();
            string description = "";

            if (categoryId.HasValue)
            {
                products = _productDao.GetByCategory(categoryId.Value);
                var category = _productDao.GetCategory(categoryId.Value);
                if (category != null)
                    description = category.Description;
            }

            var productList = products.OrderByDescending(p => p.CreatedAt).ToList();
            string html = RenderPartialViewToString("_ProductGrid", productList);

            return Json(new { html, description }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();

            var product = _productDao.GetById(id.Value);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        public ActionResult Search(string keyword)
        {
            var products = _productDao.GetAvailableProducts();

            if (!string.IsNullOrEmpty(keyword))
                products = _productDao.Search(keyword);

            var result = products.OrderByDescending(p => p.CreatedAt).ToList();

            string html = "";
            foreach (var p in result)
            {
                html += RenderPartialViewToString("_ProductCard", p);
            }

            return Json(new { html }, JsonRequestBehavior.AllowGet);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}