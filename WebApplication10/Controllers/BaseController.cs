using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;
using WebApplication10.ViewModels;

public abstract class BaseController : Controller
{
    protected TechStoreDBEntities db = new TechStoreDBEntities();
    private const string CartSessionKey = "ShoppingCart";
    protected EmailQueueDAO _emailQueueDAO;

    protected  BaseController()
    {
        db = new TechStoreDBEntities();
        _emailQueueDAO = new EmailQueueDAO(db);
    }
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // Categories (menu)
        ViewBag.Categories = db.Categories
                               .OrderBy(c => c.CategoryName)
                               .ToList();

        // Cart count
        var cart = Session[CartSessionKey] as List<CartItem>;
        ViewBag.CartCount = cart?.Sum(c => c.Quantity) ?? 0;

        // ===== ADD THIS =====
        ViewBag.ActionsModel = new AdminActionsViewModel
        {
            About = db.InfoPages.FirstOrDefault(p => p.Slug == "about"),
            Contact = db.InfoPages.FirstOrDefault(p => p.Slug == "contact"),
            Warranty = db.InfoPages.FirstOrDefault(p => p.Slug == "warranty"),
            News = db.InfoPages.FirstOrDefault(p => p.Slug == "news"),
            Careers = db.InfoPages.FirstOrDefault(p => p.Slug == "careers"),
            Returns = db.InfoPages.FirstOrDefault(p => p.Slug == "returns")
       
    };

        base.OnActionExecuting(filterContext);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
    protected void LoadAdminActions()
    {
        ViewBag.AdminActions = new AdminActionsViewModel
        {
            About = db.InfoPages.FirstOrDefault(p => p.Slug == "about"),
            Contact = db.InfoPages.FirstOrDefault(p => p.Slug == "contact"),
            Warranty = db.InfoPages.FirstOrDefault(p => p.Slug == "warranty"),
            News = db.InfoPages.FirstOrDefault(p => p.Slug == "news"),
            Careers = db.InfoPages.FirstOrDefault(p => p.Slug == "careers"),
            Returns = db.InfoPages.FirstOrDefault(p => p.Slug == "returns"),
            Shipping = db.InfoPages.FirstOrDefault(p => p.Slug == "shipping"),
            Payment = db.InfoPages.FirstOrDefault(p => p.Slug == "payment"),
            OtherPages = db.InfoPages
                .Where(p => !new[] {
                "about","contact","warranty","news",
                "careers","returns","shipping","payment"
                }.Contains(p.Slug))
                .ToList()
        };
    }
    protected void EnqueueEmail(
        string toEmail,
        string subject,
        string body,
        int? subscriberId = null,
        string emailType = "SYSTEM"
    )
    {
        _emailQueueDAO.Enqueue(new EmailQueue
        {
            ToEmail = toEmail,
            Subject = subject,
            Body = body,
            EmailType = emailType,
            Status = 0,
            RetryCount = 0,
            CreatedAt = DateTime.Now,
            SubscriberId = subscriberId
        });
    }

}
