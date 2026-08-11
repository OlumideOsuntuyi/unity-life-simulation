using UnityEngine;

namespace Simulation
{
    public class Terrain
    {
        private readonly Heightmap heightmap;
        public readonly int size;
        public Color[] colors;

        public Terrain()
        {
            size = 256;
            heightmap = new Heightmap(size);
        }
        public Terrain(int size)
        {
            this.size = size;
            heightmap = new Heightmap(size);
        }
        public Terrain(Heightmap heightmap)
        {
            size = heightmap.Size;
            this.heightmap = heightmap;
        }

        /// <summary>
        /// This function is relatively slow and should not be called per frame.
        /// </summary>
        /// <returns> Duplicate heightmap. </returns>
        public float[,] GetHeightMap()
        {
            // TODO: clone this later
            return heightmap.heightmap;
        }
    }
    [System.Serializable]
    public class Heightmap
    {
        public readonly float[,] heightmap;
        public Heightmap(int size)
        {
            heightmap = new float[size, size];
        }
        public Heightmap(float[,] heightmap)
        {
            int size = (int)Math.Min(heightmap.GetLength(0), heightmap.GetLength(1));
            this.heightmap = new float[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    this.heightmap[x, y] = heightmap[x, y];
                }
            }
        }
        /// <summary>
        /// returns size of heightmap
        /// </summary>
        public int Size
        {
            get
            {
                return heightmap.GetLength(0);
            }
        }
        /// <summary>
        /// returns size in bytes of heightmap
        /// </summary>
        public int SizeOf
        {
            get
            {
                return Size * sizeof(float);
            }
        }

        public bool Modify(float value, int x, int y)
        {
            int size = Size;
            if(x >= 0 && x < size && y >= 0 && y < size)
            {
                heightmap[x, y] = value;
                return true;
            }
            return false;
        }
    }
}