using System.Linq;
using System.Web.Mvc;
using WebApplication10.ViewModels;

namespace WebApplication10.Controllers
{
    public class AdminContactsController : BaseController
    {
        public ActionResult Index()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpStatusCodeResult(403);

            var model = new AdminContactListViewModel
            {
                Contacts = db.Contacts
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new ContactItemViewModel
                    {
                        ContactId = c.ContactId,
                        FullName = c.FullName,
                        Email = c.Email,
                        Phone = c.Phone,
                        Message = c.Message,
                        CreatedAt = c.CreatedAt
                    })
                    .ToList()
            };

            if (Request.IsAjaxRequest())
                return PartialView("_AdminContactListPartial", model);

            return View(model);
        }
    }
}