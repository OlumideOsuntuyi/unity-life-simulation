using System;
using System.Collections.Generic;

namespace Simulation.Boids
{
    public static class BoidBank
    {
        public static Dictionary<int, Boid> boid;
        private static Random Rand;
        public static void Init()
        {
            boid = new();
            Rand = new();
        }
        public static void Clear()
        {
            boid.Clear();
        }

        public static void Add(Boid bold)
        {
            boid.Add(bold.ID, bold);
        }
        public static void Remove(int ID)
        {
            boid.Remove(ID);
        }
        public static int ID()
        {
            if (Rand == null)
            {
                Rand = new(DateTime.Now.Millisecond); ;
            }
            int id;
            do
            {
                id = Rand.Next();
            } while(boid.ContainsKey(id));
            return id;
        }
    }
}