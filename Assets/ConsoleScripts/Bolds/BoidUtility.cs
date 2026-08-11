using System.Collections.Generic;

using UnityEngine;

namespace Simulation.Boids
{
    public static class BoidUtility
    {
        public static Vector3[] rayDirections { get; private set; }


        public static void Init()
        {
            GetPoints();
        }

        const float turnFraction = 55 / 34;
        public const int rayDirectionsLength = 18;
        private static void GetPoints()
        {
            rayDirections = new Vector3[rayDirectionsLength];

            List<(Vector3 dir, float dot)> dots = new();

            for (int i = 0; i < rayDirectionsLength; i++)
            {
                float phi = Mathf.Acos(1 - 2 * (i + 0.5f) / rayDirectionsLength);
                float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * (i + 0.5f);

                float x = Mathf.Sin(phi) * Math.Cos(theta);
                float y = Mathf.Sin(phi) * Math.Sin(theta);
                float z = Mathf.Cos(phi);

                rayDirections[i] = new Vector3(x, y, z);
                dots.Add((rayDirections[i], Vector3.Dot(rayDirections[i], Vector3.forward)));
            }

            dots.Sort((a, b) =>
            {
                return b.dot.CompareTo(a.dot);
            });

            for (int i = 0; i < rayDirectionsLength; i++)
            {
                rayDirections[i] = dots[i].dir;
            }

        }
    }
}