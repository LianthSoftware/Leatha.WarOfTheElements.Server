using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    /// <summary>
    /// Object representing server info.
    /// </summary>
    public sealed class ServerObject
    {
        /// <summary>
        /// Unique id of the server.
        /// </summary>
        [JsonPropertyName("serverId")]
        public int ServerId { get; set; }

        /// <summary>
        /// Name of the server.
        /// </summary>
        [JsonPropertyName("serverName")]
        public string ServerName { get; set; } = null!;

        /// <summary>
        /// Version of the server.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = null!;
    }
}
