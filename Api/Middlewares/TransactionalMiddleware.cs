using Api.Database;

namespace Api.Middlewares
{
    public class TransactionalMiddleware
    {
        private readonly RequestDelegate _next;

        public TransactionalMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DatabaseContext dbContext)
        {
            // Solo aplicamos transacción para peticiones que modifican datos
            if (context.Request.Method == HttpMethods.Post ||
                context.Request.Method == HttpMethods.Put ||
                context.Request.Method == HttpMethods.Delete)
            {
                // Iniciar transacción
                using var transaction = await dbContext.Database.BeginTransactionAsync();

                try
                {
                    await _next(context);

                    // Si no hubo errores, confirmamos los cambios
                    if (context.Response.StatusCode < 400)
                    {
                        await dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                    }
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw; // re-lanza la excepción
                }
            }
            else
            {
                await _next(context); // GET, HEAD, etc.
            }
        }
    }
}
