using UnityEngine;

namespace Simulation
{
    using Vector3 = UnityEngine.Vector3;

    public class VoxelRaycast
    {
        public static bool IsInsideSphere(UnityEngine.Vector3 position, UnityEngine.Vector3 center, float radius)
        {
            // Calculate the squared distance between the position and the center
            float distanceSquared = (position - center).sqrMagnitude;

            // Calculate the squared radius
            float radiusSquared = radius * radius;

            // Check if the squared distance is less than or equal to the squared radius
            return distanceSquared <= radiusSquared;
        }
    }
}