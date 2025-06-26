using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using Web.Models.Common;

namespace Web.Helpers
{
    public static class ModalHelpers
    {
        public static async Task<IHtmlContent> RenderMessageModalAsync(this IHtmlHelper htmlHelper)
        {
            var tempData = htmlHelper.ViewContext.TempData;
            var json = tempData["MessageModal"] as string;

            if (string.IsNullOrEmpty(json))
                return HtmlString.Empty;

            var model = JsonSerializer.Deserialize<MessageModalViewModel>(json);
            if (model == null || model.Messages == null || !model.Messages.Any())
                return HtmlString.Empty;

            return await htmlHelper.PartialAsync("_MessageModalPartial", model);
        }
    }
}
