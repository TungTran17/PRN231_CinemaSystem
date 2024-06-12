using BussinessObject.Models;
using CinemaSystemManagermentAPI.Controllers;
using DataAccess.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace CinemaSystemManagermentAPI.Filters
{
    public class AdminAuthorizationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            User user = null!;
            try
            {
                if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
                {
                    var token = headerValue.FirstOrDefault()?.Split(' ').Last();
                    if (!string.IsNullOrEmpty(token))
                    {
                        user = Authentication.GetUserByToken(token);
                    }
                    else
                    {
                        Console.WriteLine("Invalid token.");
                    }
                }
                else
                {
                    Console.WriteLine("Authorization header not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authorization error: {ex.Message}");
            }

            if (user is null || user.Role != (int)User.Roles.Admin)
            {
                context.Result = new RedirectResult("/");
            }

            if (context.Controller is AdminController adminController)
            {
                adminController.AdminUser = user!;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
