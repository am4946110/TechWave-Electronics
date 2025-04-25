using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TechWave_Electronics.Models
{
    public class AuthenticateUser : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {           
            string tempSession =
                Convert.ToString(context.HttpContext.Session.GetString("AdventureWorks.Session"));
            string tempAuthCookie =
                Convert.ToString(context.HttpContext.Request.Cookies["X-CSRF-TOKEN-HEADERNAME"]);

            if (tempSession != null && tempAuthCookie != null)
            {
                if (!tempSession.Equals(tempAuthCookie))
                {
                    ViewResult result = new ViewResult {ViewName = "Login"};
                    context.Result = result;
                }
            }
            else
            {
                ViewResult result = new ViewResult {ViewName = "Login"};
                context.Result = result;
            }
        }
    }
}
