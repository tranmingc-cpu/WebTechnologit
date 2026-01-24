using System;
using System.Web.Mvc;
using WebApplication10.DAO;

namespace WebApplication10.Controllers.Account
{
    [RoutePrefix("Newsletter")]
    public class NewsletterController : BaseController
    {
        private readonly NewsletterDAO _newsletterDao;

        public NewsletterController()
        {
            _newsletterDao = new NewsletterDAO(db);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subscribe()
        {
            if (Session["UserId"] == null)
                return new HttpStatusCodeResult(401);

            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);
            if (user == null)
                return new HttpStatusCodeResult(404);

            _newsletterDao.Add(user.Email, user.UserId, "Profile");

            return PartialView("~/Views/Shared/_NewsletterSection.cshtml", true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unsubscribe()
        {
            if (Session["UserId"] == null)
                return new HttpStatusCodeResult(401);

            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);
            if (user == null)
                return new HttpStatusCodeResult(404);

            _newsletterDao.UnsubscribeByEmail(user.Email);

            return PartialView("~/Views/Shared/_NewsletterSection.cshtml", false);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubscribeFooter(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(new { success = false });

            int? userId = Session["UserId"] != null ? (int?)Session["UserId"] : null;

            _newsletterDao.Add(email, userId, "Footer");

            return Json(new { success = true });
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult FooterStatus()
        {
            bool isSubscribed = false;

            if (Session["UserId"] != null)
            {
                int userId = (int)Session["UserId"];
                var user = db.Users.Find(userId);
                if (user != null)
                    isSubscribed = _newsletterDao.IsSubscribed(user.UserId);
            }

            return PartialView("~/Views/Shared/_NewsletterSection.cshtml", isSubscribed);
        }
    }
}
