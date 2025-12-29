using System.Linq;
using System.Web.Mvc;
using WebApplication10.DAO;
using WebApplication10.Models;
using WebApplication10.ViewModels;

public abstract class BaseController : Controller
{
    protected TechStoreDBEntities db = new TechStoreDBEntities();
    protected InfoPagesDAO _dao;

    public BaseController()
    {
        _dao = new InfoPagesDAO(db);
    }

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
        ViewBag.Categories = categories;

        var dao = new InfoPagesDAO(db);
        var allPages = dao.GetAll();

        var adminActionsModel = new AdminActionsViewModel
        {
            About = dao.GetBySlug("about"),
            Contact = dao.GetBySlug("contact"),
            Warranty = dao.GetBySlug("warranty"),
            News = dao.GetBySlug("news"),
            Careers = dao.GetBySlug("careers"),
            Returns = dao.GetBySlug("returns"),
            Shipping = dao.GetBySlug("shipping"),
            Payment = dao.GetBySlug("payment")
        };

        adminActionsModel.OtherPages = allPages
            .Where(p => !new[] { "about", "contact", "warranty", "news", "careers", "returns", "shipping", "payment" }
                         .Contains(p.Slug))
            .ToList();

        ViewBag.AdminActions = adminActionsModel;

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
}
