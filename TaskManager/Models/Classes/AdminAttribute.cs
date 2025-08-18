using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManager.Models.DbClasses;

namespace TaskManager.Models.Classes
{
    public class AdminAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session.GetString("login") == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { });
            }
            else
            {
                int loggedInId = int.Parse(context.HttpContext.Session.GetString("login"));
                MyContext dbContext = new MyContext();
                Account loggedIn = dbContext.Account.Find(loggedInId);
                if (loggedIn == null || loggedIn.LeaderId != null)
                {
                    context.Result = new RedirectToActionResult("Index", "Home", new { });
                }
            }
        }
    }
}
