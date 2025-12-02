using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Http.Headers;

namespace Web.Http
{
    public class ApiClientAuthHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiClientAuthHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Agregar el token de autorización si está disponible
            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("AccessToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Enviar la solicitud
            var response = await base.SendAsync(request, cancellationToken);

            // Manejar el caso de sesión expirada
            if (response.StatusCode == HttpStatusCode.Unauthorized &&
                       await response.Content.ReadAsStringAsync() == "Sesión expirada o iniciada en otro dispositivo")
            {
                await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                throw new UnauthorizedAccessException("La sesión ha expirado o se ha iniciado en otro dispositivo.");
            }

            return response;
        }
    }
}
