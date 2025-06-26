using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace Api.Middleware
{
    public class ModelStateValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ModelStateValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Obtiene el servicio que proporciona el contexto de la acción actual
            var actionContextAccessor = context.RequestServices.GetService<IActionContextAccessor>();

            // Verifica si el ModelState no es válido
            if (actionContextAccessor?.ActionContext?.ModelState?.IsValid == false)
            {
                var modelState = actionContextAccessor.ActionContext.ModelState;

                // Extrae todos los mensajes de error del ModelState
                var errors = modelState.Values
                          .SelectMany(v => v.Errors)
                          .Select(e => e.ErrorMessage)
                          .ToList();

                // Configura la respuesta HTTP con el código 400 y el tipo de contenido JSON
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                // Serializa la lista de errores a formato JSON
                var response = JsonSerializer.Serialize(errors);

                await context.Response.WriteAsync(response);
                return;
            }

            // Continúa con el siguiente middleware
            await _next(context);
        }
    }
}
