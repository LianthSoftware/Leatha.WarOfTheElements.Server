namespace Leatha.WarOfTheElements.Server.Objects.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EffectScriptNameAttribute : Attribute
    {
        public string Name { get; set; } = null!;
    }
}
