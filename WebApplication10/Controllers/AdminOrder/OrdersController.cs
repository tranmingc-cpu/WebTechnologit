using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;
using WebApplication10.ViewModels;

namespace WebApplication10.Controllers
{
    public class OrdersController : BaseController
    {

        public ActionResult Index()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult Details(int id)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            var order = db.Orders
     .Include("Users")
     .Include("OrderDetails")
     .Include("OrderDetails.Products")
     .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return HttpNotFound();

            return View(order);
          }
        /* public ActionResult OrdersPartial(string status, DateTime? from, DateTime? to)
         {
             var orders = db.Orders.AsQueryable();

             if (!string.IsNullOrEmpty(status))
                 orders = orders.Where(o => o.Status == status);

             if (from.HasValue)
                 orders = orders.Where(o => o.OrderDate >= from);

             if (to.HasValue)
                 orders = orders.Where(o => o.OrderDate <= to);

             return PartialView("_OrdersPartial",
                 orders.OrderByDescending(o => o.OrderDate).ToList());
         } */
        public ActionResult OrdersListPartial()
        {
            var orders = db.Orders
                           .OrderByDescending(o => o.OrderDate)
                           .ToList();

            return PartialView("_OrdersPartial", orders);
        }

    }
}
