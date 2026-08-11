using Simulation.Boids;

using UnityEngine;

using Vector3 = Simulation.Vector3;

namespace Simulation
{
    [System.Serializable]
    public class VoxelPhysics
    {
        public const float GRAVITY = 9.81f;

        public Boid bold;
        public int orientation;

        public Vector3 velocity, aceleration;
        public Quaternion rotation;
        public float drag;
        public float gravityModifier;

        public float jumpEnergy;

        public bool useGravity;
        public bool showHitbox;
        public bool jumpRequest;
        public bool sleep;
        //public bool isGrounded = false;

        public float xIncrease = 0;

        public EntityState entityState;
        public Vector3 position
        {
            get
            {
                return bold.Position;
            }
            set
            {
                bold.Position = value;
            }
        }

        public VoxelPhysics(Boid bold)
        {
            this.bold = bold;
        }
        public static float deltaTime;
        public void Update()
        {
            Vector3 XZDirection = Vector3.Cross(rotation.eulerAngles, Vector3.up).normalized;
            XZDirection.y = 0;
            if (Vector3.Angle(XZDirection, Vector3.forward) <= 45)
            {
                orientation = 0; // Player is facing forwards.
            }
            else if (Vector3.Angle(XZDirection, Vector3.right) <= 45)
            {
                orientation = 5;
            }
            else if (Vector3.Angle(XZDirection, Vector3.back) <= 45)
            {
                orientation = 1;
            }
            else
            {
                orientation = 4;
            }

            if (!sleep)
            {
                //isGrounded = bold.CheckCollision(position + (Vector3.down * 0.15f));
                ApplyVelocity();
            }
        }
        void ApplyVelocity()
        {
            Vector3 gravity = useGravity ? GRAVITY * gravityModifier * Vector3.down : Vector3.zero;

            // apply acceleration and gravity
            velocity += deltaTime * (aceleration + gravity);

            // apply drag
            velocity *= Mathf.Clamp01(1 - (drag * deltaTime));
            aceleration *= Mathf.Clamp01(1 - (drag * deltaTime));

            Vector3 dir = velocity * deltaTime;

            // translate collider
            var x = bold.TranslateDirection(Vector3.right * dir.x);
            var y = bold.TranslateDirection(Vector3.up * dir.y);
            var z = bold.TranslateDirection(Vector3.forward * dir.z);
            Vector3 displacement = (x + y + z);

            velocity = new Vector3
            {
                x = displacement.x != 0 ? 0 : velocity.x,
                y = displacement.y != 0 ? 0 : velocity.y,
                z = displacement.z != 0 ? 0 : velocity.z,
            };

            // apply bounce
            // TODO: Add bounce multiplier to block causing obstruction

            // internal function
            void ApplyFriction()
            {
                velocity = new Vector3()
                {
                    x = velocity.x - (velocity.x * Time.deltaTime * 0),
                    y = velocity.y,
                    z = velocity.z - (velocity.z * Time.deltaTime * 0),
                };
                entityState = new EntityState
                {
                    position = bold.Position,
                    rotation = rotation.eulerAngles,
                    velocity = velocity,
                    acceleration = aceleration,
                    displacement = displacement
                };
            }
            ApplyFriction();
        }
        public void AddRelativeForce(Vector3 force, ForceMode mode = ForceMode.Force)
        {
            AddForce(rotation * force, mode);
        }
        public void AddForce(Vector3 value, ForceMode mode = ForceMode.Impulse)
        {
            switch(mode)
            {
                case ForceMode.Force:
                    {
                        aceleration += value;
                    }break;
                case ForceMode.Impulse:
                    {
                        velocity += value;
                    }break;
                case ForceMode.VelocityChange:
                    {
                        velocity = value;
                    }break;
                case ForceMode.Acceleration:
                    {
                        aceleration = value;
                    }break;
                default:break;
            }
        }

        /// <summary>
        /// Rotates entity
        /// </summary>
        /// <param name="direction">direction to be rotated</param>
        public void Rotate(UnityEngine.Vector3 direction)
        {
            rotation = Quaternion.Euler(rotation.eulerAngles + direction);
        }
        /// <summary>
        /// Rotates entity
        /// </summary>
        /// <param name="angle">Angle to be rotated by.</param>
        /// <param name="axis">Axis of rotation.</param>
        public void RotateAround(float angle, Vector3 axis) => Rotate(angle * axis);
        public void LookAt(Vector3 direction, float range = 1)
        {
            rotation = Quaternion.Lerp(rotation, Quaternion.LookRotation(direction), range);
        }

        public void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            //if(Command.ShowHitbox)
            //{
            //    Gizmos.color = Color.yellow;
            //    Vector3 center = position + (collider.size * .5f);
            //    Gizmos.DrawWireCube(center, collider.size);
            //}
        }
        [System.Serializable]
        public struct EntityState
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 velocity;
            public Vector3 acceleration;
            public Vector3 displacement;
            public float friction;
            public bool isGrounded;
        }
    }
    public enum ForceMode { Force, Impulse, VelocityChange, Acceleration}
}