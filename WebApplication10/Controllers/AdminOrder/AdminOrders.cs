using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;
using WebApplication10.ViewModels;

namespace WebApplication10.Controllers
{
    [RoutePrefix("Admin/Orders")]
    public class AdminOrdersController : BaseController
    {
        private readonly OrdersDAO _orderDao;
        public AdminOrdersController()
        {
            _orderDao = new OrdersDAO(db);
        }
        // GET: Admin/Orders
        public ActionResult Index()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");
            LoadAdminActions();

            var orders = _orderDao.GetAllOrders().ToList();

            var now = DateTime.Now;

            ViewBag.TotalOrders = orders.Count;

            ViewBag.OrdersThisMonth = orders.Count(o =>
                o.OrderDate.HasValue &&
                o.OrderDate.Value.Month == now.Month &&
                o.OrderDate.Value.Year == now.Year
            );

            ViewBag.OrdersThisYear = orders.Count(o =>
                o.OrderDate.HasValue &&
                o.OrderDate.Value.Year == now.Year
            );

            return View(orders);
        }
        // GET: Admin/Orders/Details/5
        public ActionResult Details(int id)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            LoadAdminActions();

            var order = _orderDao.GetByIdWithDetails(id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        // POST: Admin/Orders/UpdateStatus
        [HttpPost]
        public ActionResult UpdateStatus(int id, string status)
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpUnauthorizedResult();

            _orderDao.UpdateStatus(id, status);

            return RedirectToAction("Details", new { id });
        }
    }
}