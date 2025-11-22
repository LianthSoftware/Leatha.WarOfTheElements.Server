using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;

namespace Leatha.WarOfTheElements.Server.Services
{
    public sealed class AuthUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var accountId = connection.User.FindFirst("account_id")?.Value;

            Debug.WriteLine(String.IsNullOrWhiteSpace(accountId)
                ? "No AccountId was provided."
                : $"Token has AccountId = \"{ accountId }\"");

            return accountId;
        }
    }
}
