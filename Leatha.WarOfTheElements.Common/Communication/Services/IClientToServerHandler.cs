using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Messages.Responses;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Common.Communication.Services
{
    public interface IClientToServerHandler
    {
        Task<TransferMessage<List<PlayerObject>>> GetCharacterList(Guid accountId);

        Task<TransferMessage<PlayerStateObject>> EnterWorld(Guid playerId); // #TODO: Server Id?

        Task<TransferMessage> ExitWorld(Guid playerId);



        Task<TransferMessage> SendPlayerInput(PlayerInputObject playerInput);



        Task<TransferMessage<PlayerObject>> GetPlayer(Guid playerId);

        Task<TransferMessage<List<SpellInfoObject>>> GetPlayerSpellBook(Guid playerId);

        Task<TransferMessage<List<SpellInfoObject>>> GetPlayerEnhancements(Guid playerId);



        Task<TransferMessage<SpellCastResult>> CastSpell(Guid casterId, int spellId);
    }
}
