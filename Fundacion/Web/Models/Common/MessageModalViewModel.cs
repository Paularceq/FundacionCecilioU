using System.Text.Json;
using Web.Enums;

namespace Web.Models.Common
{
    public class MessageModalViewModel
    {
        public IEnumerable<string> Messages { get; set; } = [];
        public MessageType Type { get; set; } = MessageType.Info;
        public string RedirectUrl { get; set; }

        public static MessageModalViewModel Error(string message, string redirectUrl = null)
            => new() { Messages = [message], Type = MessageType.Error, RedirectUrl = redirectUrl };

        public static MessageModalViewModel Error(IEnumerable<string> messages, string redirectUrl = null)
            => new() { Messages = messages, Type = MessageType.Error, RedirectUrl = redirectUrl };

        public static MessageModalViewModel Success(string message, string redirectUrl = null)
            => new() { Messages = [message], Type = MessageType.Success, RedirectUrl = redirectUrl };

        public static MessageModalViewModel Info(string message, string redirectUrl = null)
            => new() { Messages = [message], Type = MessageType.Info, RedirectUrl = redirectUrl };

        public string ToJson() => JsonSerializer.Serialize(this);
    }
}
