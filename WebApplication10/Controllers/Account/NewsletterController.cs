using System;
using System.Web.Mvc;
using WebApplication10.DAO;

namespace WebApplication10.Controllers
{
    public class NewsletterController : BaseController
    {
        private readonly NewsletterDAO _newsletterDao;

        public NewsletterController()
        {
            _newsletterDao = new NewsletterDAO(db);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subscribe(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Email không hợp lệ"
                });
            }

            if (_newsletterDao.Exists(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Email này đã đăng ký newsletter"
                });
            }

            int? userId = null;
            if (Session["UserId"] != null)
            {
                userId = (int)Session["UserId"];
            }

            try
            {
                _newsletterDao.Add(email, userId, "Footer");
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "Không thể đăng ký newsletter. Vui lòng thử lại."
                });
            }

            return Json(new
            {
                success = true,
                message = "Đăng ký nhận ưu đãi thành công 🎉"
            });
        }

        [HttpGet]
        public ActionResult Unsubscribe(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Message = "Email không hợp lệ";
                return View("UnsubscribeResult");
            }

            bool success = _newsletterDao.Unsubscribe(email);

            ViewBag.Message = success
                ? "Bạn đã hủy đăng ký nhận newsletter thành công."
                : "Email không tồn tại hoặc đã hủy trước đó.";

            return View("UnsubscribeResult");
        }
    }
}