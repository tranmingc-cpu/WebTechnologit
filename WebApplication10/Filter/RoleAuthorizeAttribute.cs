using System.Web.Mvc;

namespace WebApplication10.Filters
{
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _role;

        public RoleAuthorizeAttribute(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            if (session["UserId"] == null)
            {
                filterContext.Result =
                    new RedirectResult("/Account/Login");
                return;
            }

            var userRole = session["UserRole"]?.ToString();

            if (userRole != _role)
            {
                filterContext.Result =
                    new RedirectResult("/Account/AccessDenied");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}