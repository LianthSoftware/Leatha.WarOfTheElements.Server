namespace Leatha.WarOfTheElements.Server.Objects.Game
{
    public static class MapGroupName
    {
        public static string For(int mapId, Guid? instanceId)
        {
            if (instanceId.HasValue)
                return $"map:{ mapId }:instance:{ instanceId.Value }";

            return $"map:{ mapId }:global";
        }
    }
}
