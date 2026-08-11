using UnityEngine;

namespace Simulation.Boids
{
    public class BoidModel : MonoBehaviour
    {
        public Boid boid;
        private void Awake()
        {
            
        }
        public void Set(Boid boid)
        {
            this.boid = boid;
        }
        private void Update()
        {
            if(boid != null)
            {
                transform.position = boid.Position;
                boid.Update();
            }
        }

        public bool Raycast(Vector3 direction, float radius, float distance, out UnityEngine.RaycastHit hit)
        {
            UnityEngine.Ray ray = new (transform.position, direction);
            if(Physics.SphereCast(ray, distance, out UnityEngine.RaycastHit _hit, radius))
            {
                hit = _hit;
                BoidController.hits++;
                return true;
            }
            hit = default;
            return false;
        }

        private void OnDrawGizmos()
        {
            if(boid != null)
            {
                boid.OnDrawGizmos();
            }
        }
    }
}