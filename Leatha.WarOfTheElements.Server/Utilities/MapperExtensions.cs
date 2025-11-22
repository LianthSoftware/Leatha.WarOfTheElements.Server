using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Common.Communication.Utilities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Demo;
using Leatha.WarOfTheElements.Server.Objects.Characters;

namespace Leatha.WarOfTheElements.Server.Utilities
{
    public static class MapperExtensions
    {
        public static PlayerObject AsTransferObject(this Player entity)
        {
            return new PlayerObject
            {
                AccountId = entity.AccountId,
                PlayerId = entity.PlayerId,
                PlayerName = entity.PlayerName,
                Level = entity.Level,
                Created = entity.Created,
                PrimaryElementType = entity.PrimaryElementType,
                SecondaryElementType = entity.SecondaryElementType,
                TertiaryElementType = entity.TertiaryElementType
            };
        }

        public static ICharacterStateObject AsTransferObject(this ICharacterState entity)
        {
            if (entity.WorldObjectId.IsPlayer() && entity is PlayerState playerState)
                return AsTransferObject(playerState);

            if (entity.WorldObjectId.IsNonPlayer() && entity is NonPlayerState nonPlayerState)
                return AsTransferObject(nonPlayerState);

            return null;
        }

        public static PlayerStateObject AsTransferObject(this PlayerState entity)
        {
            return new PlayerStateObject
            {
                WorldObjectId = new WorldObjectId(entity.PlayerId, WorldObjectType.Player),
                MapId = entity.MapId,
                InstanceId = entity.InstanceId,
                Resources = entity.Resources,
                Auras = entity.Auras,
                CharacterName = entity.CharacterName,
                CharacterLevel = entity.CharacterLevel,

                // Position
                X = entity.Position.X,
                Y = entity.Position.Y,
                Z = entity.Position.Z,

                // Orientation quaternion
                Qx = entity.Orientation.X,
                Qy = entity.Orientation.Y,
                Qz = entity.Orientation.Z,
                Qw = entity.Orientation.W,

                // View angles
                Yaw = entity.Yaw,
                Pitch = entity.Pitch,

                // Velocity
                Vx = entity.Velocity.X,
                Vy = entity.Velocity.Y,
                Vz = entity.Velocity.Z,

                // State flags
                IsOnGround = entity.IsOnGround,
                IsFlying = entity.IsFlying,
                IsSprinting = entity.IsSprinting,

                LastProcessedInputSeq = entity.LastProcessedInputSeq
            };
        }

        public static NonPlayerStateObject AsTransferObject(this NonPlayerState entity)
        {
            return new NonPlayerStateObject
            {
                WorldObjectId = new WorldObjectId(entity.NonPlayerId, WorldObjectType.NonPlayer),
                TemplateId = entity.TemplateId,
                MapId = entity.MapId,
                InstanceId = entity.InstanceId,

                Resources = entity.Resources,
                Auras = entity.Auras,
                CharacterName = entity.CharacterName,
                CharacterLevel = entity.CharacterLevel,

                // Position
                X = entity.Position.X,
                Y = entity.Position.Y,
                Z = entity.Position.Z,

                // Orientation quaternion
                Qx = entity.Orientation.X,
                Qy = entity.Orientation.Y,
                Qz = entity.Orientation.Z,
                Qw = entity.Orientation.W,

                // View angles
                Yaw = entity.Yaw,
                Pitch = entity.Pitch,

                // Velocity
                Vx = entity.Velocity.X,
                Vy = entity.Velocity.Y,
                Vz = entity.Velocity.Z,

                // State flags
                IsOnGround = entity.IsOnGround,
                IsFlying = entity.IsFlying,
                IsSprinting = entity.IsSprinting,
            };
        }

        public static SpellRankObject AsTransferObject(this SpellRank entity)
        {
            return new SpellRankObject
            {
                SpellId = entity.SpellId,
                Rank = entity.Rank,
                FirstRankSpellId = entity.FirstRankSpellId,
                PreviousRankSpellId = entity.PreviousRankSpellId,
                NextRankSpellId = entity.NextRankSpellId
            };
        }

        public static SpellInfoObject AsTransferObject(this SpellTemplate entity)
        {
            return new SpellInfoObject
            {
                SpellId = entity.SpellId,
                SpellName = entity.SpellName,
                SpellDescription = entity.SpellDescription,
                CastTime = entity.CastTime,
                Cooldown = entity.Cooldown,
                Duration = entity.Duration,
                ElementChakraCosts = entity.ElementChakraCosts,
                SpellTargets = entity.SpellTargets,
                SpellFlags = entity.SpellFlags,
                SpellRank = entity.SpellRank?.AsTransferObject(),
                ElementTypes = entity.ElementTypes,
                SpellEffects = entity.SpellEffects,
                SpellIconPath = entity.SpellIconPath
            };
        }

        public static AuraInfoObject AsTransferObject(this AuraTemplate entity)
        {
            return new AuraInfoObject
            {
                AuraId = entity.AuraId,
                SpellId = entity.SpellId,
                AuraName = entity.AuraName,
                AuraDescription = entity.AuraDescription,
                Duration = entity.Duration,
                TicksCount = entity.TicksCount,
                ElementTypes = entity.ElementTypes,
                AuraFlags = entity.AuraFlags,
                AuraEffects = entity.AuraEffects,
                AuraIconPath = entity.AuraIconPath,
            };
        }

        public static MapInfoObject AsTransferObject(this MapTemplate entity)
        {
            return new MapInfoObject
            {
                MapId = entity.MapId,
                MapName = entity.MapName,
                MapDescription = entity.MapDescription,
                MapPath = entity.MapPath,
                MapSizeX = entity.MapSizeX,
                MapSizeY = entity.MapSizeY,
                MapFlags = entity.MapFlags
            };
        }
    }
}
