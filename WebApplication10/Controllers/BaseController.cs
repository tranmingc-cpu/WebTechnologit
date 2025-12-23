using System.Linq;
using System.Web.Mvc;
using WebApplication10.Models;

public abstract class BaseController : Controller
{
    protected TechStoreDBEntities db = new TechStoreDBEntities();

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var categories = db.Categories
                           .OrderBy(c => c.CategoryName)
                           .ToList();

        ViewBag.Categories = categories;

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
