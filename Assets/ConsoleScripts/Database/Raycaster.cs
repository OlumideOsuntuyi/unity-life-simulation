namespace Simulation
{
    public static class Raycaster
    {
        public const float RAYCAST_STEP = 0.1f;
        public static bool Raycast(Ray ray, out RaycastHit hit, float distance)
        {
            return Raycast(ray.origin, ray.direction, distance, out hit);
        }
        public static bool Raycast(Vector3 origin, Vector3 direction, float distance, out RaycastHit hit)
        {
            direction = direction.normalized;
            Vector3 point = origin + (RAYCAST_STEP * direction);
            float dis = Vector3.Distance(origin, point);
            while (dis < distance)
            {
                float md = float.NegativeInfinity;
                foreach(var simObjct in SimObject.objects)
                {
                    if(simObjct.collider.IsPointInCollider(point))
                    {
                        hit = new RaycastHit
                        {
                            collider = simObjct.collider,
                            point = point
                        };
                        return true;
                    }
                    else
                    {
                        md = Math.Max(md, Vector3.Distance(origin, simObjct.transform.position));
                    }
                }
                origin += RAYCAST_STEP * direction;
                dis = Vector3.Distance(origin, point);
                if(dis > md)
                {
                    break;
                }
            }
            hit = default;
            return false;
        }
    }
    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;
    }
    public struct RaycastHit
    {
        public Collider collider;
        public Vector3 point;
    }
}