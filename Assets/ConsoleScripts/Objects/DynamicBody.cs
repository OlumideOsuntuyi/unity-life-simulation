using System.Collections.Generic;
using System.Diagnostics;

namespace Simulation
{
    public class DynamicBody : Component
    {
        public Vector3 _velocity, _angularVelocity;

        public float _mass;
        public float drag, angularDrag, bounciness;
        public float mass
        {
            get
            {
                return _mass;
            }
            set
            {
                _mass = Math.Clamp(value, 0.01f, 999999);
            }
        }
        public float density
        {
            get
            {
                float d = mass / Vector3.Magnitude(gameObject.collider.size);
                if(d is >= float.PositiveInfinity or <= float.NegativeInfinity)
                {
                    d = 1;
                }
                return d;
            }
        }
        public Vector3 velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                _velocity = value;
            }
        }
        public Vector3 angularVelocity
        {
            get
            {
                return _angularVelocity;
            }
            set
            {
                _angularVelocity = value;
            }
        }

        public override void Start()
        {
            mass = 1;
            drag = 0.05f;
            angularDrag = 0.25f;
        }
        public override void Update()
        {
            float density = this.density;
            if (gameObject.collider != null)
            {
                AdjustVelocity(gameObject.collider.contacts.contacts);
                ApplyDrag(density, gameObject.collider.size, ref _velocity);
                ApplyAngularDrag(density, gameObject.collider.size, ref _angularVelocity);
                transform.position += Time.deltaTime * velocity;
                transform.rotation += Time.deltaTime * angularVelocity;
            }
        }
        private void ApplyDrag(float density, Vector3 size, ref Vector3 velocity)
        {
            float area = Area(size);
            float drag = CalculateDrag(density, this.drag, area, velocity);
            Vector3 dragForce = -drag * velocity.normalized;
            velocity += dragForce / mass;
        }
        private void ApplyAngularDrag(float density, Vector3 size, ref Vector3 angularVelocity)
        {
            float angularArea = AngularArea(size);
            float angularDrag = CalculateDrag(density, this.angularDrag, angularArea, angularVelocity);
            Vector3 angularDragTorque = -angularDrag * angularVelocity.normalized;
            angularVelocity += angularDragTorque / mass;
        }
        private float Area(Vector3 size)
        {
            return Math.Max(Math.Max(size.x * size.y, size.x * size.z), size.y * size.z);
        }
        private float AngularArea(Vector3 size)
        {
            return 2f / 5f * mass * System.MathF.Pow(Vector3.Magnitude(size), 2);
        }
        private float CalculateDrag(float density, float dragCoefficient, float area, Vector3 velocity)
        {
            float velocityMagnitudeSquared = System.MathF.Sqrt(Vector3.Magnitude(velocity));
            return 0.5f * density * dragCoefficient * area * velocityMagnitudeSquared;
        }
        public void AdjustVelocity(List<Collider.Contact> contactPoints)
        {
            foreach(var contactPoint in contactPoints)
            {
                float perpendicularVelocity = Vector3.Dot(velocity, contactPoint.normal);
                perpendicularVelocity *= -gameObject.collider.bounciness;

                Vector3 tangentialVelocity = velocity - perpendicularVelocity * contactPoint.normal;
                tangentialVelocity *= 1 - gameObject.collider.friction;
                velocity = tangentialVelocity + perpendicularVelocity * contactPoint.normal;

                Vector3 torque = Vector3.Cross(contactPoint.normal, tangentialVelocity);

                torque.x = 0;
                torque.z = 0;

                angularVelocity += torque;
            }
        }
        public void AddForce(Vector3 force)
        {
            Vector3 acceleration = (force / mass);

            Vector3 dragForce = (drag * velocity);

            Vector3 newVelocity = velocity + ((acceleration - dragForce) * Time.deltaTime);
            velocity = newVelocity;
        }
        public void AddRelativeForce(Vector3 force)
        {

        }
        public void AddTorque(Vector3 torque)
        {
            Vector3 acceleration = (torque / mass);

            Vector3 dragForce = (drag * angularVelocity);

            Vector3 newVelocity = angularVelocity + ((acceleration - dragForce) * Time.deltaTime);
            angularVelocity = newVelocity;
        }

        [System.Serializable]
        public struct Info
        {
            public Vector3 velocity, angularVelocity;
            public bool isAsleep;
        }
    }
}