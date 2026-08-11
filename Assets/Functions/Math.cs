using System;

namespace Simulation
{
    public struct Math
    {
        public const float Deg2Rad = 0.0145329f;
        public const float Rad2Deg = 57.2957795f;
        public const float PI = 3.142159274f;

        private static Random Rand;

        public static float Random(float min, float max)
        {
            return RandomInt((int)(min * 1000), (int)(max * 1000)) / 1000f;
        }
        public static int RandomInt(int min_, int max_)
        {
            if(Rand == null)
            {
                Rand = new(DateTime.Now.Millisecond); ;
            }
            int min = (int)Min(min_, max_);
            int max = (int)Max(min_, max_);    
            return Rand.Next(min, max + 1);
        }
        public static float Abs(float value)
        {
            return value >= 0 ? value : value * -1;
        }
        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }
        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }
        public static float Clamp(float value, float min, float max)
        {
            return Max(min, Min(max, value));
        }
        public static float Lerp(float a, float b, float range)
        {
            return (1 - range) * a + (range * b);
        }
        public static float Sin(float radians)
        {
            return (float)System.Math.Sin(radians);
        }
        public static float Cos(float radians)
        {
            return (float)System.Math.Cos(radians);
        }
        public static float Tan(float radians)
        {
            return (float)System.Math.Tan(radians);
        }
        public static float ToDegrees(float radians)
        {
            return radians * Rad2Deg;
        }
        public static float ToRadians(float degrees)
        {
            return degrees * Deg2Rad;
        }
        public static float Sqrt(float value)
        {
            return System.MathF.Sqrt(value);
        }
    }
}