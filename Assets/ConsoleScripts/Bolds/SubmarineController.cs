using UnityEngine;


namespace Simulation.Boids
{
    using Vector3 = UnityEngine.Vector3;
    using Time = UnityEngine.Time;

    public class SubmarineController : MonoBehaviour
    {
        public float speed;
        public float turnSpeed;
        public float drag, angularDrag;
        public Vector3 velocity, angularVelocity;

        private void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            angularVelocity += Time.deltaTime * turnSpeed * new UnityEngine.Vector3(v, h, 0);

            if (Input.GetMouseButton(0))
            {
                velocity += transform.rotation * (Time.deltaTime * speed * Vector3.forward);
            }

            transform.position += Time.deltaTime * velocity;
            transform.Rotate(Time.deltaTime * angularVelocity);

            velocity = (1 - (drag * Time.deltaTime)) * velocity;
            angularVelocity = (1 - (angularDrag * Time.deltaTime)) * angularVelocity;
        }
    }
}