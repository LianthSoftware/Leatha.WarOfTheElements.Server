using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using System.Diagnostics;

namespace Leatha.WarOfTheElements.Server.Scripts.Auras
{
    public abstract class AuraScriptBase
    {
        //protected NonPlayerScriptBase()
        //{
        //}

        protected AuraScriptBase(AuraObject auraObject, AuraTemplate template) // : this()
        {
            AuraObject = auraObject;
            Template = template;
        }

        public AuraObject AuraObject { get; init; }

        public AuraTemplate Template { get; init; }

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



        //public void OnTargetHit(ICharacterState target)
        //{
        //    Debug.WriteLine($"[Script]: OnTargetHit -> {target.WorldObjectId.ObjectId}");
        //}
    }
}
