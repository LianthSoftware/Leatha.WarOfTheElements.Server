using System.Collections.Concurrent;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IPresenceTracker
    {
        Task TrackConnected(string playerId, string connectionId);

        Task TrackDisconnected(string playerId, string connectionId);

        //bool IsOnline(string playerId);

        IReadOnlyCollection<string> GetConnections(string accountId);
    }

    public sealed class PresenceTracker : IPresenceTracker
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _connections =
            new(StringComparer.OrdinalIgnoreCase);

        public Task TrackConnected(string accountId, string connectionId)
        {
            var set = _connections.GetOrAdd(accountId, _ => []);
            lock (set)
            {
                set.Add(connectionId);
            }
            return Task.CompletedTask;
        }

        public Task TrackDisconnected(string accountId, string connectionId)
        {
            if (_connections.TryGetValue(accountId, out var set))
            {
                lock (set)
                {
                    set.Remove(connectionId);
                    if (set.Count == 0)
                        _connections.TryRemove(accountId, out _);
                }
            }
            return Task.CompletedTask;
        }

        //public bool IsOnline(string accountId)
        //    => _connections.ContainsKey(accountId);

        public IReadOnlyCollection<string> GetConnections(string accountId) =>
            _connections.TryGetValue(accountId, out var set) ? set : [];
    }
}
