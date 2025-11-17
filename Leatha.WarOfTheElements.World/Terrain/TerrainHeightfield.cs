using System.Numerics;

namespace Leatha.WarOfTheElements.World.Terrain
{
    public sealed class TerrainHeightfield
    {
        public Vector3 Origin { get; }

        public float CellSize { get; }

        public float[,] Heights { get; }

        public TerrainHeightfield(Vector3 origin, float cellSize, float[,] heights)
        {
            Origin = origin;
            CellSize = cellSize;
            Heights = heights;
        }
    }
}
