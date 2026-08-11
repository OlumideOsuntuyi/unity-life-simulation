using System.Collections.Generic;

using UnityEngine;

namespace Simulation.Boids
{
    using Vector3Int = UnityEngine.Vector3Int;
    using Transform = UnityEngine.Transform;

    public class BoidController : Singleton<BoidController>
    {
        BoidChunks boids;
        public int amount = 20;
        public int count, chunkCount;
        public static int collisions, hits;
        public int _collisions, _hits;
        public GameObject prefab;
        public Transform parent;
        public float size, displayScale;

        [SerializeField] private BoidSimulationSettings settings;
        public static BoidSimulationSettings Settings;
        public static float Bound => Instance.size - 2;
        private void Awake()
        {
            Settings = settings;
        }
        private void Start()
        {
            BoidBank.Init();
            boids = new();

            BoxCollider[] children = parent.gameObject.GetComponentsInChildren<BoxCollider>();
            List<UnityEngine.Vector3> directions = new() { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right, Vector3Int.forward, Vector3Int.back };
            for (int dir = 0; dir < 6; dir++)
            {
                children[dir].transform.position = (size * -.5f) * directions[dir];
                children[dir].transform.localScale = new UnityEngine.Vector3
                {
                    x = directions[dir].x == 0 ? size : 1,
                    y = directions[dir].y == 0 ? size : 1,
                    z = directions[dir].z == 0 ? size : 1
                };
            }

        }
        private void Update()
        {
            if(Boid.Count < amount)
            {
                boids.Create();
            }
            BoidChunks.Update();
            VoxelPhysics.deltaTime = UnityEngine.Time.deltaTime;
            count = Boid.Count;

            Settings = settings;
        }
        private void LateUpdate()
        {
            _collisions = collisions;
            collisions = 0;

            _hits = hits;
            hits = 0;
        }
    }

    [System.Serializable]
    public struct BoidSimulationSettings
    {
        public float cohesionDistance;
        public float flockDistance;
        public float avoidanceDistance;
        public bool constrainDimension;

        [Range(PositionFunctions.SIZE, PositionFunctions.SIZE * (BoidChunks.CHUNK_SIZE - 1))] public int Bound;

        [Range(0f, 50f)] public float speed;
        [Range(-1f, 1f)] public float view;

        [Range(0f, 1f)] public float cohesionStrength;
        [Range(0f, 1f)] public float flockStrength;
        [Range(0f, 1f)] public float avoidanceStrength;
    }
}