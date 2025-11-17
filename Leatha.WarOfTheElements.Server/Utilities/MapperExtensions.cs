using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Demo;
using Leatha.WarOfTheElements.Server.Objects.Game;

namespace Leatha.WarOfTheElements.Server.Utilities
{
    public static class MapperExtensions
    {
        public static PlayerObject AsTransferObject(this Player entity)
        {
            return new PlayerObject
            {
                PlayerId = entity.PlayerId,
                PlayerName = entity.PlayerName,
                Created = entity.Created
            };
        }

        public static PlayerStateObject AsTransferObject(this PlayerState entity)
        {
            return new PlayerStateObject
            {
                PlayerId = entity.PlayerId,
                MapId = entity.MapId,
                InstanceId = entity.InstanceId,

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
                SpellTargets = entity.SpellTargets,
                SpellFlags = entity.SpellFlags,
                SpellRank = entity.SpellRank?.AsTransferObject(),
                ElementTypes = entity.ElementTypes,
                SpellEffectType = entity.SpellEffectType,
                SpellIconPath = entity.SpellIconPath
            };
        }
    }
}
