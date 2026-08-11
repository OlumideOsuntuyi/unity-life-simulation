using System.Collections.Generic;

using UnityEngine;

namespace Simulation.Boids
{
    public class BoidChunks
    {
        public const int CHUNK_SIZE = 16;
        private readonly Vector3Int SIZE = new Vector3Int(1, 1, 1) * CHUNK_SIZE;
        private static Dictionary<Vector3Int, BoidChunk> chunks;
        private static List<Vector3Int> keys;
        public BoidChunks()
        {
            BoidUtility.Init();
            chunks = new();
            Populate();
        }
        private void Populate()
        {
            keys = new();
            for (int x = -CHUNK_SIZE; x < CHUNK_SIZE; x++)
            {
                for (int y = -CHUNK_SIZE; y < CHUNK_SIZE; y++)
                {
                    for (int z = -CHUNK_SIZE; z < CHUNK_SIZE; z++)
                    {
                        if(BoidController.Settings.constrainDimension)
                        {
                            if (z != 0) continue;
                        }
                        Vector3Int position = new(x, y, z);
                        BoidChunk chunk = new(position);
                        chunks.Add(position, chunk);
                        keys.Add(position);
                        BoidController.Instance.chunkCount++;
                    }
                }
            }
        }

        public void Create()
        {
            Vector3 axis = BoidController.Settings.constrainDimension ? Vector3.XY : Vector3.one;
            Vector3 position = Vector3.Rand(-BoidController.Bound, BoidController.Bound, axis);

            BoidModel model = UnityEngine.Object.Instantiate(BoidController.Instance.prefab, BoidController.Instance.transform).AddComponent<BoidModel>();
            model.transform.forward = new(RandFloat(), RandFloat(), RandFloat());

            Boid boid = new(model);
            boid.Init(position);
            model.Set(boid);
        }
        public float RandFloat()
        {
            return Random.Range(-1.00f, 1.00f);
        }

        public static BoidChunk Get(ref Vector3Int chunk)
        {
            return chunks[chunk];
        }
        public static bool ValidChunk(ref Vector3Int chunk)
        {
            return chunk.x is >= 0 and < CHUNK_SIZE &&
                chunk.y is >= 0 and < CHUNK_SIZE &&
                chunk.z is >= 0 and < CHUNK_SIZE;
        }
        public static bool ValidatePosition(Vector3 position)
        {
            Vector3Int chunk = PositionFunctions.FromWorld(position);
            if(chunks.ContainsKey(chunk))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void Update()
        {
            foreach (var key in keys)
            {
                chunks[key].Update();
            }
        }
        public void OnDrawGizmos()
        {
            if(chunks.Count >= CHUNK_SIZE)
            {
                Gizmos.color = new Color(.5f, .1f, .5f);
                Vector3 size = PositionFunctions.SIZE * Vector3.one;
                Vector3 centerOffset = size * -.5f;
                foreach (var chunk in chunks.Keys)
                {
                    Vector3 pos = centerOffset + chunk;
                    Gizmos.DrawWireCube(pos, size);
                }
            }
        }
    }

    public class BoidChunk
    {
        private readonly Vector3Int position;
        private readonly List<int> bolds;
        public readonly List<int> Content;
        public BoidChunk(Vector3Int position)
        {
            this.position = position;
            bolds = new();
            Content = new();
        }
        public void Add(Boid bold)
        {
            bolds.Add(bold.ID);
        }
        public void Remove(Boid bold)
        {
            bolds.Remove(bold.ID);
        }
        public void Update()
        {
            Content.Clear();
            Vector3Int final;
            for (byte i = 0; i < 7; i++)
            {
                final = position + (directions[i] * PositionFunctions.SIZE);
                if (BoidController.Settings.constrainDimension)
                {
                    if (final.z != 0) continue;
                }
                if (BoidChunks.ValidChunk(ref final))
                {
                    Content.AddRange(BoidChunks.Get(ref final).bolds);
                }
            }
        }
        private static readonly List<Vector3Int> directions = new() { Vector3Int.zero, Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right, Vector3Int.forward, Vector3Int.back };
    }
}