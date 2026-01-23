using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;
using WebApplication10.Services;

namespace WebApplication10.Controllers.Admin
{
    public class CategoriesController : BaseController
    {
        private CategoryDAO _dao;

        public CategoriesController()
        {
            db = new TechStoreDBEntities();
            _dao = new CategoryDAO(db);
        }

        public ActionResult Description(int id)
        {
            var model = _dao.GetById(id);
            if (model == null)
                return HttpNotFound();

            ViewBag.CurrentCategoryId = id;

            if (Request.IsAjaxRequest())
                return PartialView("_CategoryDescriptionPartial", model);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpStatusCodeResult(403);

            var model = _dao.GetById(id);
            if (model == null)
                return HttpNotFound();

            if (Request.IsAjaxRequest())
                return PartialView("_EditCategoryDescriptionPartial", model);

            return new HttpStatusCodeResult(400);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "CategoryId,Description")] Categories model)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return Json(new { success = false, errors = new[] { "Unauthorized" } });

            try
            {
                _dao.UpdateDescription(model.CategoryId, model.Description);

                return Json(new
                {
                    success = true,
                    message = "Cập nhật mô tả danh mục thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { ex.Message }
                });
            }
        }
    }
}
