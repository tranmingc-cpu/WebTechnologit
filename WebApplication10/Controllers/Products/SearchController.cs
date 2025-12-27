using System.Linq;
using System.Web.Mvc;
using WebApplication10.Models;
using WebApplication10.DAO;

namespace WebApplication10.Controllers.Products
{
    public class SearchController : BaseController
    {
        private readonly ProductDao _productDao;

        public SearchController()
        {
            _productDao = new ProductDao(db);
        }

        public ActionResult QuickSearch(string keyword)
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
