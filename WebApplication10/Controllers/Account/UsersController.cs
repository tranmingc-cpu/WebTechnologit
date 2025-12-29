using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication10.Models;
using WebApplication10.DAO;

namespace WebApplication10.Controllers
{
    public class UsersController : BaseController
    {
        private readonly UserDao _userDao;

        public UsersController()
        {
            _userDao = new UserDao(new TechStoreDBEntities());
        }

        public ActionResult Index()
        {
            return View(); 
        }


        public ActionResult UserListPartial()
        {
            var users = _userDao.GetAll();
            return PartialView("_UsersPartial", users);
        }


        public ActionResult Create()
        {
            return PartialView("_CreateUser");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Users user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors });
            }

            try
            {
                _userDao.Add(user);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { ex.Message } });
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = _userDao.GetById(id.Value);
            if (user == null)
                return HttpNotFound();

            return PartialView("_EditUser", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Users user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors });
            }

            try
            {
                _userDao.Update(user);
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, errors = new[] { ex.Message } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { ex.Message } });
            }
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = _userDao.GetById(id.Value);
            if (user == null)
                return HttpNotFound();

            return PartialView("_DeleteUser", user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (_userDao.Delete(id, out string message))
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message });
        }

    }
}