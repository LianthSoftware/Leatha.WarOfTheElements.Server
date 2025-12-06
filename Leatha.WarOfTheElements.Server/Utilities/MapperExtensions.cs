using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Common.Communication.Utilities;
using Leatha.WarOfTheElements.Common.Environment.Collisions;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Objects.GameObjects;
using Leatha.WarOfTheElements.Server.Objects.Spells;

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

                Position = entity.Position,
                Orientation = entity.Orientation,
                Velocity = entity.Velocity,

                // View angles
                Yaw = entity.Yaw,
                Pitch = entity.Pitch,

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
                WorldObjectId = entity.WorldObjectId,
                TemplateId = entity.TemplateId,
                MapId = entity.MapId,
                InstanceId = entity.InstanceId,

                Resources = entity.Resources,
                Auras = entity.Auras,
                CharacterName = entity.CharacterName,
                CharacterLevel = entity.CharacterLevel,

                Position = entity.Position,
                Orientation = entity.Orientation,
                Velocity = entity.Velocity,

                // View angles
                Yaw = entity.Yaw,
                Pitch = entity.Pitch,

                // State flags
                IsOnGround = entity.IsOnGround,
                IsFlying = entity.IsFlying,
                IsSprinting = entity.IsSprinting,
            };
        }

        public static GameObjectStateObject AsTransferObject(this GameObjectState entity)
        {
            return new GameObjectStateObject
            {
                WorldObjectId = new WorldObjectId(entity.GameObjectId, WorldObjectType.GameObject),
                TemplateId = entity.TemplateId,
                MapId = entity.MapId,
                InstanceId = entity.InstanceId,

                Auras = entity.Auras,

                Position = entity.Position,
                Orientation = entity.Orientation,

                GameObjectName = entity.GameObjectName,
                NodeName = entity.NodeName,
            };
        }

        public static ProjectileStateObject AsTransferObject(this ProjectileState entity)
        {
            return new ProjectileStateObject
            {
                ProjectileId = entity.ProjectileId,
                SpellGuid = entity.SpellGuid,
                CasterId = entity.CasterId,
                SpellId = entity.SpellId,
                MapId = entity.MapId,
                InstanceId = entity.InstanceId,
                Position = entity.Position,
                Orientation = entity.Orientation,
                Velocity = entity.Velocity,
                RemainingLifetimeMs = entity.RemainingLifetimeMs,
                Launched = entity.Launched
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
                SpellIconPath = entity.SpellIconPath,
                VisualSpellPath = entity.VisualSpellPath
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

        public static GameObjectInfoObject AsTransferObject(this GameObjectTemplate entity)
        {
            return new GameObjectInfoObject
            {
                GameObjectId = entity.GameObjectId,
                Name = entity.Name,
                SceneName = entity.SceneName,
            };
        }














        public static ColliderArchetypeObject AsTransferObject(this ColliderArchetype entity)
        {
            return new ColliderArchetypeObject
            {
                Name = entity.Name,
                ArchetypeName = entity.ArchetypeName,
                ShapeType = entity.ShapeType,
                Size = entity.Size,
                HullVertices = entity.HullVertices,
                IsStaticDefault = entity.IsStaticDefault
            };
        }

        public static EnvironmentInstanceObject AsTransferObject(this EnvironmentInstance entity)
        {
            return new EnvironmentInstanceObject
            {
                Name = entity.Name,
                ArchetypeName = entity.ArchetypeName,
                ShapeType = entity.ShapeType,
                ColliderSize = entity.ColliderSize,
                Position = entity.Position,
                ConvexHullPoints = entity.ConvexHullPoints,
                MapId = entity.MapId,
                IsStatic = entity.IsStatic,
                RotationDegrees = entity.RotationDegrees
            };
        }

        public static EnvironmentInstance FromTransferObject(this EnvironmentInstanceObject obj)
        {
            return new EnvironmentInstance
            {
                Name = obj.Name,
                ArchetypeName = obj.ArchetypeName,
                ShapeType = obj.ShapeType,
                ColliderSize = obj.ColliderSize,
                Position = obj.Position,
                ConvexHullPoints = obj.ConvexHullPoints,
                MapId = obj.MapId,
                IsStatic = obj.IsStatic,
                RotationDegrees = obj.RotationDegrees,
            };
        }
    }
}
