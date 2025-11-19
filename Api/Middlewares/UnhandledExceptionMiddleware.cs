using System.Text.Json;

namespace Api.Middlewares
{
    public class UnhandledExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UnhandledExceptionMiddleware> _logger;

        public UnhandledExceptionMiddleware(RequestDelegate next, ILogger<UnhandledExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Registra el error no controlado con detalles de la excepción
                _logger.LogError(ex, "Unhandled exception occurred.");

                // Devuelve un código de estado 500 (Error interno del servidor)
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // Especifica que la respuesta será en formato JSON
                context.Response.ContentType = "application/json";

                // Crea una lista de errores genéricos para no exponer detalles sensibles al cliente
                var errors = new List<string> { "¡Ups! Algo salió mal. Por favor, inténtalo de nuevo más tarde." };

                // Serializa la lista de errores a JSON
                var response = JsonSerializer.Serialize(errors);

                // Escribe la respuesta serializada en el cuerpo de la respuesta HTTP
                await context.Response.WriteAsync(response);
            }
        }
    }

}
