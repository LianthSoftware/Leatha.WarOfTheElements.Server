using System.Collections.Concurrent;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IPresenceTracker
    {
        Task TrackConnected(string playerId, string connectionId);

        Task TrackDisconnected(string playerId, string connectionId);

        bool IsOnline(string playerId);

        IReadOnlyCollection<string> GetConnections(string playerId);
    }

    public sealed class PresenceTracker : IPresenceTracker
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _connections =
            new(StringComparer.OrdinalIgnoreCase);

        public Task TrackConnected(string playerId, string connectionId)
        {
            var set = _connections.GetOrAdd(playerId, _ => []);
            lock (set) set.Add(connectionId);
            return Task.CompletedTask;
        }

        public Task TrackDisconnected(string playerId, string connectionId)
        {
            if (_connections.TryGetValue(playerId, out var set))
            {
                lock (set)
                {
                    set.Remove(connectionId);
                    if (set.Count == 0)
                        _connections.TryRemove(playerId, out _);
                }
            }
            return Task.CompletedTask;
        }

        public bool IsOnline(string playerId)
            => _connections.ContainsKey(playerId);

        public IReadOnlyCollection<string> GetConnections(string playerId) =>
            _connections.TryGetValue(playerId, out var set) ? set : [];
    }
}
