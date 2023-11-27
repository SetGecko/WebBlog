using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using WebBlog.Extensions;

namespace WebBlog.Filters
{
    public class UserActivityFilter : IActionFilter
    {
        private readonly ILogger<UserActivityFilter> _logger;

        public UserActivityFilter(ILogger<UserActivityFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Получаем имя пользователя и имя контроллера
            string? userName = context.HttpContext.User.Identity?.Name;
            string controllerName = context.Controller.GetType().Name;

            //// Получаем имя действия
            //string? actionName = context.ActionDescriptor?.DisplayName;

            //// Получаем передаваемые параметры
            //string parameters = GetActionParameters(context.HttpContext);

            // Логируем завершение выполнения действия с информацией о пользователе, контроллере и результате выполнения
            _logger.RecordUserAction($"User {userName} started executing action {context.ActionDescriptor?.DisplayName} in controller {controllerName}");

           
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Получаем имя пользователя и имя контроллера
            string? userName = context.HttpContext.User.Identity?.Name;
            string controllerName = context.Controller.GetType().Name;

            // Получаем имя действия
            string? actionName = context.ActionDescriptor.DisplayName;

            // Получаем передаваемые параметры
            string parameters = GetActionParameters(context.HttpContext);

            // Логируем начало выполнения действия с информацией о пользователе, контроллере и параметрах
            _logger.RecordUserAction($"User {userName} started executing action {actionName} in controller {controllerName} with parameters: {parameters}" );
        }

        /// <summary>
        /// Возвращает параметры передаваемые запрашиваемому методу
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static string GetActionParameters(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var routeValues = httpContext.Request.RouteValues;

            if (endpoint?.Metadata.GetMetadata<ActionDescriptor>() is ActionDescriptor actionDescriptor)
            {
                var parameterNames = actionDescriptor.Parameters.Select(p => p.Name);
                var parameters = routeValues.Where(rv => parameterNames.Contains(rv.Key)).ToDictionary(rv => rv.Key, rv => rv.Value);
                return string.Join(", ", parameters.Select(p => $"{p.Key} = {p.Value}"));
            }

            return string.Empty;
        }
    }
}
