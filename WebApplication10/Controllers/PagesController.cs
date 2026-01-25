using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;
using System.Collections.Generic;
using WebApplication10.ViewModels;

namespace WebApplication10.Controllers
{
    public class PagesController : BaseController
    {
        private InfoPagesDAO _dao;

        public PagesController()
        {
            db = new TechStoreDBEntities();
            _dao = new InfoPagesDAO(db);
        }

        public ActionResult Index(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return HttpNotFound();

            var model = _dao.GetBySlug(slug);
            if (model == null)
                return HttpNotFound();
            ViewBag.CurrentPageSlug = slug;
            if (Request.IsAjaxRequest())
                return PartialView("_PageContentPartial", model);
            return View(model);
        }
        public ActionResult Contact()
        {
            var pageContent = _dao.GetBySlug("contact");
            if (pageContent == null)
                return HttpNotFound();

            ViewBag.CurrentPageSlug = "contact";

            var model = new ContactPageViewModel
            {
                PageContent = pageContent,
                ContactForm = new ContactFormViewModel()
            };

            AutoFillContactEmail(model.ContactForm);

            ViewBag.Title = pageContent.Title ?? "Liên hệ với chúng tôi";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactPageViewModel model)
        {
            AutoFillContactEmail(model.ContactForm);

            if (Request.IsAjaxRequest())
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Vui lòng kiểm tra lại thông tin."
                    });
                }

                var contact = new Contacts
                {
                    FullName = model.ContactForm.FullName,
                    Email = model.ContactForm.Email,
                    Phone = model.ContactForm.Phone,
                    Message = model.ContactForm.Message,
                    CreatedAt = DateTime.Now
                };

                db.Contacts.Add(contact);
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Cảm ơn bạn đã gửi liên hệ! Chúng tôi sẽ phản hồi sớm."
                });
            }

            if (!ModelState.IsValid)
            {
                model.PageContent = _dao.GetBySlug("contact");
                AutoFillContactEmail(model.ContactForm);
                return View(model);
            }

            var normalContact = new Contacts
            {
                FullName = model.ContactForm.FullName,
                Email = model.ContactForm.Email,
                Phone = model.ContactForm.Phone,
                Message = model.ContactForm.Message,
                CreatedAt = DateTime.Now
            };

            db.Contacts.Add(normalContact);
            db.SaveChanges();

            TempData["Success"] = "Cảm ơn bạn đã gửi liên hệ!";
            return RedirectToAction("Contact");
        }

        public ActionResult ContactPartial()
        {
            var pageContent = _dao.GetBySlug("contact");
            if (pageContent == null)
                return HttpNotFound();

            var model = new ContactPageViewModel
            {
                PageContent = pageContent,
                ContactForm = new ContactFormViewModel()
            };

            AutoFillContactEmail(model.ContactForm);

            return PartialView("_ContactPartial", model);
        }

        public ActionResult Edit(int? id, string slug)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpStatusCodeResult(403);

            InfoPages model = null;

            if (id.HasValue)
                model = _dao.GetById(id.Value);
            else if (!string.IsNullOrEmpty(slug))
                model = _dao.GetBySlug(slug);

            if (model == null)
                return HttpNotFound();

            if (Request.IsAjaxRequest())
                return PartialView("_EditPagePartial", model);

            return new HttpStatusCodeResult(400);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] 
        public ActionResult Edit([Bind(Include = "InfoPageId,Title,Content")] InfoPages model)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
            {
                return Json(new { success = false, errors = new[] { "Unauthorized" } });
            }

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            try
            {
                _dao.Update(model);

                return Json(new
                {
                    success = true,
                    message = $"Cập nhật trang '{model.Title}' thành công!"
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

        public ActionResult List()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            var pages = db.InfoPages.OrderBy(p => p.Title).ToList();

            ViewBag.Pages = pages;

            return View(pages);
        }

        public List<InfoPages> GetAllPages()
        {
            return db.InfoPages.OrderBy(p => p.Title).ToList();
        }
        private void AutoFillContactEmail(ContactFormViewModel form)
        {
            if (Session["UserEmail"] != null && string.IsNullOrEmpty(form.Email))
            {
                form.Email = Session["UserEmail"].ToString();
            }
        }

    }
}
