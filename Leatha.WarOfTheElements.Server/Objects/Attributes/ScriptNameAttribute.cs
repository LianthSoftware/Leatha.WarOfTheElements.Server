namespace Leatha.WarOfTheElements.Server.Objects.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ScriptNameAttribute : Attribute
    {
        public ScriptNameAttribute(string name, ScriptType scriptType)
        {
            Name = name;
            ScriptType = scriptType;
        }

        public string Name { get; init; }

        public ScriptType ScriptType { get; init; }
    }

    public enum ScriptType
    {
        None                            = 0,
        NonPlayer                       = 1,
        Spell                           = 2,
        Aura                            = 3,
        AreaTrigger                     = 4,
    }
}
