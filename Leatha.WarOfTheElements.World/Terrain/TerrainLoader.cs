using System.Numerics;
using System.Text.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Leatha.WarOfTheElements.World.Terrain
{
    public static class TerrainLoader
    {
        public static TerrainHeightfield Load(string metaPath, string heightPngPath, float maxTerrainHeight)
        {
            var metaJson = File.ReadAllText(metaPath);
            var meta = JsonSerializer.Deserialize<TerrainMeta>(metaJson)
                       ?? throw new InvalidOperationException("Failed to deserialize terrain meta.");

            using var image = Image.Load<Rgba32>(heightPngPath);

            var width = image.Width;   // X
            var height = image.Height; // Z

            // Sanity check: meta vs PNG
            if (meta.Width != 0 && meta.Height != 0 &&
                (meta.Width != width || meta.Height != height))
            {
                throw new InvalidOperationException(
                    $"Terrain meta size ({meta.Width}x{meta.Height}) doesn't match heightmap PNG ({width}x{height}).");
            }

            var heights = new float[width, height];

            for (var z = 0; z < height; z++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixel = image[x, z];

                    // 0..255 -> 0..1
                    var normalized = pixel.R / 255f;
                    var terrainHeight = normalized * maxTerrainHeight;

                    heights[x, z] = terrainHeight;
                }
            }

            var origin = new Vector3(meta.OriginX, meta.OriginY, meta.OriginZ);
            return new TerrainHeightfield(origin, meta.CellSize, heights);
        }

    }
}