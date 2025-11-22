using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using System.Diagnostics;

namespace Leatha.WarOfTheElements.Server.Scripts.Spells
{
    public abstract class SpellScriptBase
    {
        //protected NonPlayerScriptBase()
        //{
        //}

        protected SpellScriptBase(SpellObject spellObject, SpellTemplate template) // : this()
        {
            SpellObject = spellObject;
            Template = template;
        }

        public SpellObject SpellObject { get; init; }

        public SpellTemplate Template { get; init; }

        public List<WorldObjectId> Targets { get; init; } = [];




        public virtual void OnInitialize()
        {
        }

        public virtual void OnReset()
        {
        }

        public virtual void OnUpdate(double delta)
        {
        }



        public void OnTargetHit(ICharacterState target)
        {
            Debug.WriteLine($"[Script]: OnTargetHit -> { target.WorldObjectId.ObjectId }");
        }
    }
}
