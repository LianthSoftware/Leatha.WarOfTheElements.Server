using System.Threading.Channels;
using Leatha.WarOfTheElements.Common.Communication.Transfer;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IInputQueueService
    {
        //void Enqueue(PlayerInputObject input);

        //bool TryDequeue(out PlayerInputObject? input);

        //IAsyncEnumerable<PlayerInputObject> ReadAllAsync(CancellationToken ct);

        void Enqueue(PlayerInputObject message);

        void Drain(Action<List<PlayerInputObject>> consumer);
    }

    public sealed class InputQueueService : IInputQueueService
    {
        private readonly object _lock = new();
        private readonly List<PlayerInputObject> _buffer = [];

        public void Enqueue(PlayerInputObject message)
        {
            lock (_lock)
            {
                _buffer.Add(message);
            }
        }

        public void Drain(Action<List<PlayerInputObject>> consumer)
        {
            List<PlayerInputObject> copy;
            lock (_lock)
            {
                if (_buffer.Count == 0)
                    return;

                copy = new List<PlayerInputObject>(_buffer);
                _buffer.Clear();
            }

            consumer(copy);
        }

        //private readonly Channel<PlayerInputObject> _channel =
        //    Channel.CreateUnbounded<PlayerInputObject>(new UnboundedChannelOptions
        //    {
        //        SingleReader = false,
        //        SingleWriter = false
        //    });

        //public void Enqueue(PlayerInputObject input)
        //{
        //    _channel.Writer.TryWrite(input);
        //}

        //public bool TryDequeue(out PlayerInputObject? input)
        //{
        //    if (_channel.Reader.TryRead(out var value))
        //    {
        //        input = value;
        //        return true;
        //    }

        //    input = null;
        //    return false;
        //}

        //public IAsyncEnumerable<PlayerInputObject> ReadAllAsync(CancellationToken ct)
        //    => _channel.Reader.ReadAllAsync(ct);
    }
}
