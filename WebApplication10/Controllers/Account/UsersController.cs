using System;
using System.Net;
using System.Web.Mvc;
using WebApplication10.Models;
using WebApplication10.DAO;
using WebApplication10.ViewModels;

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

        [HttpGet]
        public ActionResult Create()
        {
            return PartialView("_CreateUser", new AdminCreateUserVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminCreateUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateUser", model);
            }

            if (_userDao.ExistsByEmail(model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                return PartialView("_CreateUser", model);
            }

            try
            {
                var user = new Users
                {
                    Username = model.Username,
                    Password = model.Password,
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Role = model.Role,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _userDao.Add(user);

                return Json(new { success = true, message = "Tạo người dùng thành công!" });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo người dùng.");
                return PartialView("_CreateUser", model);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = _userDao.GetById(id.Value);
            if (user == null)
                return HttpNotFound();

            var vm = new AdminEditUserVM
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role
            };

            return PartialView("_EditUser", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminEditUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditUser", model);
            }

            var user = _userDao.GetById(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "Người dùng không tồn tại.");
                return PartialView("_EditUser", model);
            }

            if (_userDao.ExistsByEmail(model.Email, model.UserId))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng bởi tài khoản khác.");
                return PartialView("_EditUser", model);
            }

            try
            {
                user.Username = model.Username;
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.Phone = model.Phone;
                user.Role = model.Role;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    user.Password = model.Password;
                }

                _userDao.Update(user);

                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật người dùng.");
                return PartialView("_EditUser", model);
            }
        }

        [HttpGet]
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
        public ActionResult DeleteConfirmed(int userId)
        {
            if (_userDao.Delete(userId, out string message))
            {
                return Json(new { success = true, message = "Xóa người dùng thành công!" });
            }

            return Json(new { success = false, message });
        }
    }
}