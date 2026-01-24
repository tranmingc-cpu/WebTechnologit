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

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
        ViewBag.Categories = categories;

        // Add cart count to ViewBag
        var cart = Session[CartSessionKey] as List<CartItem>;
        ViewBag.CartCount = cart?.Sum(c => c.Quantity) ?? 0;

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

}
