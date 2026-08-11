using System.Numerics;

namespace Simulation
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return value is null or "";
        }
        public static Quaternion Euler(Vector3 vector)
        {
            return Euler(vector.x, vector.y, vector.z);
        }
        public static Quaternion Euler(float x, float y, float z)
        {
            float angleX = x * Math.Deg2Rad;
            float angleY = y * Math.Deg2Rad;
            float angleZ = z * Math.Deg2Rad;

            float halfX = angleX * .5f;
            float halfY = angleY * .5f;
            float halfZ = angleZ * .5f;

            float cosX = (float)System.Math.Cos(halfX);
            float sinX = (float)System.Math.Sin(halfX);
            float cosY = (float)System.Math.Cos(halfY);
            float sinY = (float)System.Math.Sin(halfY);
            float cosZ = (float)System.Math.Cos(halfZ);
            float sinZ = (float)System.Math.Sin(halfZ);

            float x1 = (cosY * cosX * cosZ) + (sinY * sinX * sinZ);
            float y1 = (cosY * sinX * cosZ) + (sinY * cosX * sinZ);
            float z1 = (sinY * cosX * cosZ) - (cosY * sinX * sinZ);
            float w1 = (cosY * cosX * sinZ) - (sinY * sinX * cosZ);

            return new Quaternion(x1, y1, z1, w1);
        }
    }
}