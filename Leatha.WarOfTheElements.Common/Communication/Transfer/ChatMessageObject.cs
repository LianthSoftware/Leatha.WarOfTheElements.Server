using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class ChatMessageObject
    {
        public WorldObjectId TalkerId { get; set; }

        public string TalkerName { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChatMessageType MessageType { get; set; }

        public string PlainMessage { get; set; } = null!;

        public string FormattedMessage { get; set; } = null!;

        public float Duration { get; set; }

        public string TextColor { get; set; } = ColorTranslator.ToHtml(Color.White);

        public DateTime SentOn { get; set; } = DateTime.UtcNow;
    }

    public enum ChatMessageType
    {
        Say                             = 0,
        Yell                            = 1,
        Mutter                          = 2,
        Whisper                         = 3,
    }
}
