using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Transfer;

namespace Leatha.WarOfTheElements.Common.Communication.Services
{
    public interface IServerToClientHandler
    {
        //Task SendSnapshot(WorldSnapshotMessage message);

        Task SendSnapshot(WorldSnapshotMessage message, CancellationToken cancellationToken);
    }
}
