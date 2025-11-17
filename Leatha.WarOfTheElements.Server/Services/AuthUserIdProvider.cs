using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;

namespace Leatha.WarOfTheElements.Server.Services
{
    public sealed class AuthUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var playerId = connection.User.FindFirst("player_id")?.Value;

            Debug.WriteLine(String.IsNullOrWhiteSpace(playerId)
                ? "No PlayerId was provided."
                : $"Token has PlayerId = \"{playerId}\"");

            return playerId;
        }
    }
}
