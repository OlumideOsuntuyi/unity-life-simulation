
using UnityEngine;

namespace Simulation
{

    public class TerrainGenerator
    {
        FastNoise noise, erosionNoise;
        TerrainGenerationSettings settings;
        public TerrainGenerator(TerrainGenerationSettings settings)
        {
            noise = new(settings.seed);
            erosionNoise = new(new System.Random(settings.seed).Next());
            this.settings = settings;
        }
        public void SetSettings(TerrainGenerationSettings settings)
        {
            this.settings = settings;
        }
        public Terrain Generate(Vector3 position, out Color[] colors)
        {
            int size = 1025;
            int index = 0;
            float height;

            colors = new Color[1024 * 1024];
            Heightmap map = new(size);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    height = Generate(x, y, position);
                    map.Modify(height, x, y);
                    if(x < 1024 && y < 1024)
                    {
                        colors[index] = settings.colors.Evaluate(Mathf.Clamp01(height));
                        index++;
                    }
                }
            }
            Terrain terrain = new(map);
            return terrain;
        }

        /// <summary>
        /// Changes -1 to 1 range of noise to 0 to 1
        /// </summary>
        /// <param name="noise"></param>
        /// <returns> clamped value</returns>
        public static float Clamp(ref float noise)
        {
            return (noise + 1) * .5f;
        }
        public float Generate(float x, float y, Vector3 position)
        {
            x += .5f + position.x;
            y += .5f + position.z;


            x *= settings.scale;
            y *= settings.scale;

            noise.SetFractalType(settings.fractalType);
            noise.SetFractalOctaves(settings.fractals);
            float continental = noise.GetPerlinFractal(x, y);
            Clamp(ref continental);
            continental = settings.continentalCurve.Evaluate(continental) * settings.continentalHeight;


            erosionNoise.SetFractalType(settings.erosionType);
            erosionNoise.SetFractalOctaves(settings.erosionFractals);
            float erosion = erosionNoise.GetCubicFractal(y * settings.erosionScale, x * settings.erosionScale);
            Clamp(ref erosion);
            erosion = settings.erosionCurve.Evaluate(erosion) * settings.erosionHeight;

            float height = (1 - settings.continentalHeight) + continental - erosion;

            return height;
        }
    }

    [System.Serializable]
    public struct TerrainGenerationSettings
    {
        public int seed;
        public float scale;
        public int fractals, erosionFractals;
        public FastNoise.FractalType fractalType, erosionType;

        public float erosionScale;

        [Range(0f, 1f)] public float continentalHeight;
        [Range(0f, 1f)] public float erosionHeight;

        public AnimationCurve continentalCurve, erosionCurve;

        public Gradient colors;
    }
}