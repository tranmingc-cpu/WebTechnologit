using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;
using WebApplication10.Services;
using WebApplication10.ViewModels;

namespace WebApplication10.Controllers
{
    public class AdminController : BaseController
    {
        private readonly AdminDao _adminDao;
        private readonly EmailQueueProcessor _emailQueueProcessor;


        public AdminController()
        {
            db = new TechStoreDBEntities();

            _adminDao = new AdminDao(db);

            _emailQueueProcessor = new EmailQueueProcessor(db);
        }

        public ActionResult Dashboard(bool partial = false)
        {
            ViewBag.IsPartial = partial;
            // Chưa đăng nhập
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Không phải admin
            if (Session["UserRole"]?.ToString() != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.IsPartial = partial;
            LoadAdminActions();

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            var chartData = _adminDao.GetRevenueByYear(year);

            var model = new DashboardViewModel
           /* {
                UserCount = _adminDao.GetUserCount(),
                OrderCount = _adminDao.GetOrderCount(),
                ProductCount = _adminDao.GetProductCount(),
                CurrentMonthRevenue = _adminDao.GetRevenueByMonth(year, month),
                Months = Enumerable.Range(1, 12).ToList(),
                Revenues = Enumerable.Range(1, 12)
                    .Select(m =>
                        (decimal?)(
                            chartData.FirstOrDefault(x => x.Month == m)?.CurrentMonthRevenue ?? 0
                        )
                    )
                    .ToList()
            };

            if (partial)
                return PartialView("_Dashboard", model);

            return View(model);
        } */


            {
                     UserCount = 120,
                     OrderCount = 340,
                     ProductCount = 58,

                     CurrentMonthRevenue = 12500000,

                     // test chart
                     Months = new List<int> { 1, 2, 3, 4, 5, 6 },
                     Revenues = new List<decimal?>
          {
              8000000,
              9500000,
              7200000,
              11000000,
              9800000,
              12500000
          }
                 };

                 if (partial)
                     return PartialView("_Dashboard", model);

                 return View(model);

             }
  

        public ActionResult AdminActionsPartial()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpUnauthorizedResult();

            var about = db.InfoPages.FirstOrDefault(p => p.Slug == "about");
            var contact = db.InfoPages.FirstOrDefault(p => p.Slug == "contact");
            var warranty = db.InfoPages.FirstOrDefault(p => p.Slug == "warranty");
            var news = db.InfoPages.FirstOrDefault(p => p.Slug == "news");
            var careers = db.InfoPages.FirstOrDefault(p => p.Slug == "careers");
            var returns = db.InfoPages.FirstOrDefault(p => p.Slug == "returns");
            var shipping = db.InfoPages.FirstOrDefault(p => p.Slug == "shipping");
            var payment = db.InfoPages.FirstOrDefault(p => p.Slug == "payment");

            var otherPages = db.InfoPages
                .Where(p => !new[] { "about", "contact", "warranty", "news", "careers", "returns", "shipping", "payment" }.Contains(p.Slug))
                .ToList();

            var model = new AdminActionsViewModel
            {
                About = about,
                Contact = contact,
                Warranty = warranty,
                News = news,
                Careers = careers,
                Returns = returns,
                Shipping = shipping,
                Payment = payment,
                OtherPages = otherPages
            };

            return PartialView("_AdminActions", model);
        }
        public ActionResult RunEmailQueue()
        {
            _emailQueueProcessor.Process(20);
            return Content("OK");
        }
     


    }


}