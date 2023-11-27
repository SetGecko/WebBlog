using Microsoft.AspNetCore.Authorization;

namespace WebBlog.Extensions
{
    /// <summary>
    /// расширениея для класса AuthorizationHandlerContextExtensions 
    /// </summary>
    internal static class AuthorizationHandlerContextExtensions
    {
        /// <summary>
        /// Возвращает значение для указанного ключа
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string? GetRouteValue(this AuthorizationHandlerContext context, string key)
        {
            if (context.Resource is HttpContext httpContext)
            {
                var routeValue = httpContext.Request.RouteValues[key];
                if (routeValue != null)
                {
                    return routeValue.ToString();
                }
            }
            return null;
        }
    }
}
