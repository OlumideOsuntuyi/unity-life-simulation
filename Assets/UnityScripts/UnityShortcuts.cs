using UnityEngine;

namespace Simulation.Unity
{
    public static class UnityShortcuts
    {
        public static void ClearTransform(UnityEngine.Transform transform)
        {
            foreach(UnityEngine.Transform child in transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }
        public static UnityEngine.Vector3 ToUnity(this Simulation.Vector3 value)
        {
            return new UnityEngine.Vector3(value.x, value.y, value.z);
        }
    }
}