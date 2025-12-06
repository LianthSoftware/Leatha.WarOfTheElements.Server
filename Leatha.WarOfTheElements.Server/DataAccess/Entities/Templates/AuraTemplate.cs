using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class AuraTemplate : MongoEntity
    {
        public int AuraId { get; set; }

        public int SpellId { get; set; }

        public string AuraName { get; set; } = null!;

        public string AuraDescription { get; set; } = null!;

        public ElementTypes ElementTypes { get; set; }

        public int Duration { get; set; } // Milliseconds.

        public int TicksCount { get; set; }

        public AuraFlags AuraFlags { get; set; }



        public List<AuraEffectObject> AuraEffects { get; set; } = [];

        public string AuraIconPath { get; set; } = null!;

        public string? ScriptName { get; set; }
    }
}
