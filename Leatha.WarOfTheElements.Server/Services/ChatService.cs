using System.Diagnostics;
using System.Drawing;
using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Objects.Game;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IChatService
    {
        Task<TransferMessage<ChatMessageObject>> TalkAsync(
            string message,
            ChatMessageType messageType,
            float duration,
            ICharacterState state,
            List<ICharacterState> players);
    }

    public sealed class ChatService : IChatService
    {
        public ChatService(IServerToClientHandler serverClient)
        {
            _serverToClientHandler = serverClient;
        }

        private readonly IServerToClientHandler _serverToClientHandler;

        private const string ChatItemTemplate = "[p][color=4ac4ff][{0}][/color] [color=gray][{1}][/color]: [color={2}]{3}[/color][/p]";

        public async Task<TransferMessage<ChatMessageObject>> TalkAsync(
            string message,
            ChatMessageType messageType,
            float duration,
            ICharacterState state,
            List<ICharacterState> players)
        {
            //var distance = 50.0f;

            //var players = _gameWorld.GetCharacters(state.Position, distance);

            var chatMessage = new ChatMessageObject
            {
                TalkerId = state.WorldObjectId,
                TalkerName = state.CharacterName,
                MessageType = messageType,
                PlainMessage = message,
                TextColor = ColorTranslator.ToHtml(GetChatMessageColor(messageType)),
                Duration = duration
            };

            Debug.WriteLine("Color = " + chatMessage.TextColor);

            chatMessage.FormattedMessage = FormatMessage(chatMessage);

            await _serverToClientHandler.Talk(chatMessage,
                players
                    .OfType<PlayerState>()
                    .Select(i => i.AccountId)
                    .ToList());

            return TransferMessage.CreateMessage(chatMessage);
        }

        private string FormatMessage(ChatMessageObject chatMessage)
        {
            var message = string.Format(
                ChatItemTemplate,
                chatMessage.SentOn.ToString("HH:mm:ss"),
                chatMessage.TalkerName,
                chatMessage.TextColor,
                chatMessage.PlainMessage);

            return message;
        }

        public static Color GetChatMessageColor(ChatMessageType type)
        {
            switch (type)
            {
                case ChatMessageType.Say:
                    break;
                case ChatMessageType.Yell:
                    return ColorTranslator.FromHtml("#7f1900");
                case ChatMessageType.Mutter:
                    return ColorTranslator.FromHtml("#656565");
                case ChatMessageType.Whisper:
                    return Color.MediumPurple;
            }

            return Color.White;
        }
    }
}
