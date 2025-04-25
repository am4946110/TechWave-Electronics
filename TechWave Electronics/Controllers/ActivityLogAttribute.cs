using TechWave_Electronics.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;

namespace TechWave_Electronics.Controllers
{
    public class ActivityLogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var request = context.HttpContext.Request;
            var user = context.HttpContext.User;

            
            var userName = user.Identity?.Name ?? "Anonymous";

           
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);
            
            var rolesString = roles.Any() ? string.Join(", ", roles) : "Not Role";

            var log = new ActivityLog
            {
                UserName = userName,
                IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                UrlAccessed = request.Path,
                RolseUser = rolesString,
                Timestamp = DateTime.Now.AddHours(12)
            };

            string formattedTime = log.Timestamp.AddHours(12).ToString("dd/MMM/yyyy   hh:mm   t");

            // الحصول على DbContext من خلال خدمات التطبيق
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            dbContext.ActivityLogs.Add(log);
            dbContext.SaveChanges();

            base.OnActionExecuted(context);
        }
    }
}
