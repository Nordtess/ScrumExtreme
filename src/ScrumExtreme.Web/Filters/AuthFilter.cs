using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ScrumExtreme.Web.Filters;

public class AuthFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString();

        // Allow the login page through without authentication
        if (string.Equals(controllerName, "Login", StringComparison.OrdinalIgnoreCase))
            return;

        var role = context.HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role))
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
