using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Transfer;

namespace Leatha.WarOfTheElements.Common.Communication.Services
{
    public interface IServerToClientHandler
    {
        //Task SendSnapshot(WorldSnapshotMessage message);

        Task SendSnapshot(WorldSnapshotMessage message, CancellationToken cancellationToken);

        Task SendSpellStart(SpellObject spellObject, ICharacterStateObject caster);

        Task SendSpellFinished(SpellObject spellObject, ICharacterStateObject caster);

        Task SendAuraApply(AuraObject auraObject, ICharacterStateObject target);

        Task SendAuraRemove(AuraObject auraObject, ICharacterStateObject target);

        Task Talk(ChatMessageObject chatMessage, List<Guid> accountIds);

        Task SetGameObjectState(SetGameStateMessage message, int mapId, Guid? instanceId);

        //Task ActivateGameObject(GameObjectStateObject gameObject, int mapId, Guid? instanceId);


        Task PlayerEnteredMap(PlayerStateObject playerState);
    }
}
