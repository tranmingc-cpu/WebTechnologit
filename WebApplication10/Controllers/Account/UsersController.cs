using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication10.Models;

namespace WebApplication10.Controllers
{
    public class UsersController : Controller
    {
        private TechStoreDBEntities _context = new TechStoreDBEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        // GET: Users Partial List (for AJAX)
        public ActionResult UserListPartial()
        {
            var users = _context.Users.ToList();
            return PartialView("_UserList", users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return PartialView("_CreateUser");
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Users user)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values
                                             .SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList();
                return Json(new { success = false, errors = modelErrors });
            }

            user.CreatedAt = DateTime.Now;
            user.IsActive = true;
            if (string.IsNullOrWhiteSpace(user.Role))
                user.Role = "Customer";

            _context.Users.Add(user);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Users user = _context.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            return PartialView("_EditUser", user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Users user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors = errors });
            }

            var existingUser = _context.Users.Find(user.UserId);
            if (existingUser == null)
                return Json(new { success = false, errors = new[] { "User not found." } });

            existingUser.Username = user.Username;
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.Address = user.Address;
            existingUser.Role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role;

 
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                existingUser.Password = user.Password;
            }

            try
            {
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors
                               .SelectMany(eve => eve.ValidationErrors)
                               .Select(ve => ve.PropertyName + ": " + ve.ErrorMessage)
                               .ToList();
                return Json(new { success = false, errors = errors });
            }
        }


        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Users user = _context.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            return PartialView("_DeleteUser", user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                if (user.Orders.Any() || user.Cart.Any() || user.Reviews.Any())
                {
                    return Json(new { success = false, message = "Cannot delete user with related data." });
                }

                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
