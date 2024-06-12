using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using CinemaSystemManagermentAPI.Controllers;

namespace CinemaSystemManagermentAPI.Filters
{
    public class AdminAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null && endpoint.Metadata.GetMetadata<AdminAuthorizationFilter>() != null)
            {
                var adminAuthorizationFilter = context.RequestServices.GetRequiredService<AdminAuthorizationFilter>();
                var actionExecutingContext = new ActionExecutingContext(
                    new ActionContext { HttpContext = context, RouteData = context.GetRouteData() },
                    new List<IFilterMetadata>(),
                    new Dictionary<string, object>(),
                    new DummyController()
                );

                adminAuthorizationFilter.OnActionExecuting(actionExecutingContext);

                if (actionExecutingContext.Result != null)
                {
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }
            await _next(context);
        }

    }
}
