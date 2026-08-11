namespace Simulation
{
    public class Locomotion
    {
        public void MoveToTarget(Animal life, Vector3 target)
        {
            if(life.data.status.Stamina() == 0)
            {
                UnityEngine.Debug.Log($"No stamina");
                return;
            }
            life.data.status.UseStamina();
            float force = life.data.genes.modifications.speed * Benchmarks.Instance.benchmark.speed;

            Vector3 direction = target - life.gameObject.transform.position;

            life.gameObject.rigidBody.AddForce(direction.normalized * force);

            life.gameObject.rigidBody.velocity = Vector3.ClampMagnitude(life.gameObject.rigidBody.velocity, force);

            life.transform.LookAt(target, 2 * Time.deltaTime);
        }
        public void TurnTowardsTarget(Animal life, Vector3 target)
        {
            Vector3 currentPosition= life.gameObject.transform.position;
            float turnSpeed = life.data.genes.modifications.speed;

            Vector3 direction = target - currentPosition;

            if (direction != Vector3.zero)
            {
                Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction).normalized;

                float angle = System.MathF.Acos(Vector3.Dot(Vector3.forward, direction.normalized)) * Math.Rad2Deg;


                float torqueMagnitude = System.MathF.Min(turnSpeed, angle);
                Vector3 torque = rotationAxis * torqueMagnitude;
                life.gameObject.rigidBody.AddTorque(torque);
            }
        }


    }
}