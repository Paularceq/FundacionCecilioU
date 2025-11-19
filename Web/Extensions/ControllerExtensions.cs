using Microsoft.AspNetCore.Mvc;
using Web.Models.Common;

namespace Web.Extensions
{
    /// <summary>
    /// Métodos de extensión para controladores ASP.NET Core MVC que facilitan la configuración de mensajes modales (error, éxito, información) con redirección opcional.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Establece un mensaje de error para mostrar en un modal, con redirección opcional después de cerrar el modal.
        /// </summary>
        /// <param name="controller">Instancia del controlador.</param>
        /// <param name="message">Mensaje de error a mostrar.</param>
        /// <param name="redirectAction">Acción a la que redirigir después de cerrar el modal (opcional).</param>
        /// <param name="redirectController">Controlador al que redirigir después de cerrar el modal (opcional).</param>
        public static void SetErrorMessage(this Controller controller, string message, string redirectAction = null, string redirectController = null)
        {
            string redirectUrl = GetRedirectUrl(controller, redirectAction, redirectController);
            var modal = MessageModalViewModel.Error(message, redirectUrl);
            controller.TempData["MessageModal"] = modal.ToJson();
        }

        /// <summary>
        /// Establece múltiples mensajes de error para mostrar en un modal, con redirección opcional después de cerrar el modal.
        /// </summary>
        /// <param name="controller">Instancia del controlador.</param>
        /// <param name="messages">Colección de mensajes de error a mostrar.</param>
        /// <param name="redirectAction">Acción a la que redirigir después de cerrar el modal (opcional).</param>
        /// <param name="redirectController">Controlador al que redirigir después de cerrar el modal (opcional).</param>
        public static void SetErrorMessage(this Controller controller, IEnumerable<string> messages, string redirectAction = null, string redirectController = null)
        {
            string redirectUrl = GetRedirectUrl(controller, redirectAction, redirectController);
            var modal = MessageModalViewModel.Error(messages, redirectUrl);
            controller.TempData["MessageModal"] = modal.ToJson();
        }

        /// <summary>
        /// Establece un mensaje de éxito para mostrar en un modal, con redirección opcional después de cerrar el modal.
        /// </summary>
        /// <param name="controller">Instancia del controlador.</param>
        /// <param name="message">Mensaje de éxito a mostrar.</param>
        /// <param name="redirectAction">Acción a la que redirigir después de cerrar el modal (opcional).</param>
        /// <param name="redirectController">Controlador al que redirigir después de cerrar el modal (opcional).</param>
        public static void SetSuccessMessage(this Controller controller, string message, string redirectAction = null, string redirectController = null)
        {
            string redirectUrl = GetRedirectUrl(controller, redirectAction, redirectController);
            var modal = MessageModalViewModel.Success(message, redirectUrl);
            controller.TempData["MessageModal"] = modal.ToJson();
        }

        /// <summary>
        /// Establece un mensaje informativo para mostrar en un modal, con redirección opcional después de cerrar el modal.
        /// </summary>
        /// <param name="controller">Instancia del controlador.</param>
        /// <param name="message">Mensaje informativo a mostrar.</param>
        /// <param name="redirectAction">Acción a la que redirigir después de cerrar el modal (opcional).</param>
        /// <param name="redirectController">Controlador al que redirigir después de cerrar el modal (opcional).</param>
        public static void SetInfoMessage(this Controller controller, string message, string redirectAction = null, string redirectController = null)
        {
            string redirectUrl = GetRedirectUrl(controller, redirectAction, redirectController);
            var modal = MessageModalViewModel.Info(message, redirectUrl);
            controller.TempData["MessageModal"] = modal.ToJson();
        }

        /// <summary>
        /// Genera una URL de redirección basada en los nombres de acción y controlador proporcionados.
        /// </summary>
        /// <param name="controller">Instancia del controlador.</param>
        /// <param name="redirectAction">Acción a la que redirigir.</param>
        /// <param name="redirectController">Controlador al que redirigir. Si es nulo, se usa el controlador actual.</param>
        /// <returns>La URL generada, o null si no se especifica acción.</returns>
        private static string GetRedirectUrl(Controller controller, string redirectAction, string redirectController = null)
        {
            if (string.IsNullOrEmpty(redirectAction))
                return null;

            if (string.IsNullOrEmpty(redirectController))
                redirectController = controller.ControllerContext.RouteData.Values["controller"].ToString();

            return controller.Url.Action(redirectAction, redirectController);
        }
    }
}