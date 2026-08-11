using System.Collections.Generic;

using UnityEngine;

namespace Simulation.Boids
{
    using Transform = UnityEngine.Transform;

    [System.Serializable]
    public class Boid
    {
        public static BoidSimulationSettings Settings => BoidController.Settings;
        public float AvoidanceDistance => Settings.avoidanceDistance;
        public float FlockDistance => Settings.flockDistance;
        public float CohesionDistance => Mathf.Min(Settings.cohesionDistance, FlockDistance);

        private const float Radius = 2f;

        public readonly int ID;

        public static int Count { get; private set; }

        public VoxelPhysics.EntityState state;

        BoidModel model;
        Transform transform => model.transform;
        VoxelPhysics physics;


        Vector3 _position;
        Vector3Int _chunk;

        Vector3 velocity;
        Vector3 acceleration;
        List<NeighbourData> neighbours;

        public Vector3Int Chunk
        {
            get
            {
                return _chunk;
            }
            private set
            {
                if(_chunk != value)
                {
                    BoidChunks.Get(ref _chunk).Remove(this);
                    BoidChunks.Get(ref value).Add(this);
                    _chunk = value;
                }
            }
        }
        public BoidChunk CHUNK => BoidChunks.Get(ref _chunk);
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (Settings.constrainDimension) value.z = 0;
                if (value.x <= -BoidController.Bound || value.x >= BoidController.Bound)
                {
                    value.x = BoidController.Bound * -(Mathf.Sign(value.x));
                }
                if (value.y <= -BoidController.Bound || value.y >= BoidController.Bound)
                {
                    value.y = BoidController.Bound * -(Mathf.Sign(value.y));
                }
                if (value.z <= -BoidController.Bound || value.z >= BoidController.Bound)
                {
                    value.z = BoidController.Bound * -(Mathf.Sign(value.z));
                }
                if (_position != value)
                {
                    if(BoidChunks.ValidatePosition(value))
                    {
                    }
                    else
                    {
                    }
                    Chunk = PositionFunctions.FromWorld(value);
                    _position = value;
                }
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return physics.rotation;
            }
        }



        public Boid(BoidModel model)
        {
            ID = BoidBank.ID();
            this.model = model;

            physics = new(this);
            physics.gravityModifier = 0;

            BoidBank.Add(this);
            Count++;

            neighbours = new();
        }

        public void Init(Vector3 spawnPosition)
        {
            _position = spawnPosition;
            _chunk = PositionFunctions.FromWorld(Position);
            BoidChunks.Get(ref _chunk).Add(this);
        }

        public void Destroy()
        {
            BoidBank.Remove(ID);
            Count--;
        }
        public void Update()
        {
            if(model)
            {
                var content = CHUNK.Content;
                Boid boid;

                List<NeighbourData> neighbours = new();

                Vector3 forward = transform.forward;
                Vector3 initialVelocity = forward;
                Vector3 flockVelocities = new();
                Vector3 cohesionVelocities = new();
                Vector3 avoidanceVelocities = new();

                int flockCount = 0;
                int viewCount = 0;
                int avoidances = 0;

;
                foreach (var i in content)
                {
                    if (i != ID)
                    {
                        boid = BoidBank.boid[i];
                        float distance = Vector3.Distance(boid.Position, Position);
                        if (distance < FlockDistance)
                        {
                            Vector3 neighbourForward = boid.transform.forward;
                            Vector3 directionToNeighbour = (boid.Position - Position).normalized;
                            float viewDot = Vector3.Dot(directionToNeighbour, forward);


                            if (distance < AvoidanceDistance)
                            {
                                avoidanceVelocities -= directionToNeighbour;
                                avoidances++;
                            }
                            else
                            {
                                flockVelocities += neighbourForward;
                                flockCount++;
                                if (viewDot >= Settings.view)
                                {
                                    cohesionVelocities += directionToNeighbour;
                                    viewCount++;
                                }
                                // add velocity due to avoidance
                                // if too close do not add to flock calculation
                                else
                                {

                                }
                            }
                            neighbours.Add(new NeighbourData
                            {
                                distance = distance,
                                forward = neighbourForward,
                                dot = Vector3.Dot(forward, neighbourForward),
                                position = boid.Position,
                                id = boid.ID
                            });
                        }
                    }
                }
                
                this.neighbours = neighbours;
                if(neighbours.Count > 0 && flockCount > 0)
                {
                    if (neighbours.Count > 1)
                    {
                        neighbours.Sort((a, b) =>
                        {
                            return a.distance.CompareTo(b.distance);
                        });
                    }

                    Vector3 flockCenter = cohesionVelocities / flockCount;
                    Vector3 flockDirection = flockVelocities / flockCount;

                    Vector3 to_nearest_center = (flockCenter - Position).normalized;

                    Vector3 averageForward = (flockDirection + to_nearest_center) * .5f;
                    transform.forward = averageForward;
                }

                model.gameObject.name = $"{StaticShortcuts.AddCommasToNumber(neighbours.Count)} neighbors.";

                Vector3 velocity = initialVelocity * Settings.speed;
                if (CollidingForward())
                {
                    transform.forward = FindUnobstructedDirection();
                    velocity = Settings.speed * forward;
                }
                else if(avoidances > 0)
                {
                    Vector3 meanAvoidanceVelocity = avoidanceVelocities / Mathf.Max(1f, avoidances);
                    //meanAvoidanceVelocity *= Settings.avoidanceStrength;

                    velocity = meanAvoidanceVelocity * Settings.speed;

                }
                else if(flockCount > 0)
                {
                    Vector3 meanFlockVelocity = flockVelocities / Mathf.Max(1f, flockCount);
                    meanFlockVelocity *= Settings.flockStrength;

                    Vector3 meanCohesionVelocity = cohesionVelocities / Mathf.Max(1f, viewCount);
                    meanCohesionVelocity *= Settings.cohesionStrength;

                    velocity = meanFlockVelocity + meanCohesionVelocity;
                    velocity *= Settings.speed;
                }

                transform.LookAt(Position + velocity);
                physics.AddForce(velocity, ForceMode.VelocityChange);
            }

            physics.Update();
            state = physics.entityState;
        }
        public void HandleInteraction(Boid bold)
        {

        }

        private Vector3 FindUnobstructedDirection()
        {
            Vector3 bestDir = transform.forward;
            float furthestUnobstructedDst = 0;

            for (int i = 0; i < BoidUtility.rayDirectionsLength; i++)
            {
                Vector3 dir = transform.TransformDirection(BoidUtility.rayDirections[i]);
                if (Settings.constrainDimension) dir.z = 0;

                bool hit = false;
                if(Raycast(dir, Radius, out float hitDistance))
                {
                    hit = true;
                }

                if (model.Raycast(dir, Radius, AvoidanceDistance, out UnityEngine.RaycastHit hit_))
                {
                    hit = true;
                    hitDistance = Mathf.Min(hitDistance, hit_.distance);
                }
                if(!hit && furthestUnobstructedDst <= hitDistance)
                {
                    bestDir = dir;
                    furthestUnobstructedDst = hitDistance;
                }
            }

            return transform.InverseTransformDirection(bestDir);
        }
        private bool Raycast(Vector3 direction, float distance, out float hitDistance)
        {
            hitDistance = 0;
            while(hitDistance < distance)
            {
                if (Colliding(Position + (direction * hitDistance)))
                {
                    return true;
                }
                hitDistance += Radius;
            }
            return false;
        }
        private bool RaycastModel(Vector3 dir)
        {
            return model.Raycast(dir, Radius, AvoidanceDistance, out var hit);
        }
        private bool Colliding(Vector3 point)
        {
            Vector3Int chunk = PositionFunctions.FromWorld(point);
            if (BoidChunks.ValidChunk(ref chunk))
            {
                var content = BoidChunks.Get(ref chunk).Content;
                foreach (var b in content)
                {
                    if (b != ID)
                    {
                        BoidController.collisions++;
                        Boid boid = BoidBank.boid[b];
                        if (CheckCollision(point, boid.Position, Radius))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }
        private bool CheckCollision(Vector3 a, Vector3 b, float radius)
        {
            return Vector3.Distance(a, b) < radius;
        }


        public Vector3 TranslateDirection(Vector3 direction)
        {
            Vector3 targetPosition = Position + direction;
            if (Colliding(targetPosition))
            {
                return direction;
            }
            Position = targetPosition;
            return Vector3.zero;
        }

        public bool CollidingForward()
        {
            if (model.Raycast(transform.forward, Radius, AvoidanceDistance, out UnityEngine.RaycastHit hit))
            {
                return true;
            }
            return false;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = RaycastModel(transform.forward) ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * AvoidanceDistance));

            float invFlk = 1f / FlockDistance;
            foreach(var neighbour in neighbours)
            {
                float dis = 1f - (neighbour.distance * invFlk);
                Gizmos.color = new Color(.2f, .2f, 1f, dis);
                Gizmos.DrawLine(Position, neighbour.position);
            }
        }

        private struct NeighbourData
        {
            public int id;
            public float dot;
            public float distance;
            public Vector3 forward;
            public Vector3 position;
        }
    }
}