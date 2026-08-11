using System;

namespace Simulation
{
    [System.Serializable]
    public struct Vector3Int : IEquatable<Vector3Int>
    {
        public int x;
        public int y;
        public int z;
        public static Vector3Int zero => new(0, 0, 0);
        public static Vector3Int one => new(1, 1, 1);
        public static Vector3Int forward => new(0, 0, 1);
        public static Vector3Int back => new(0, 0, -1);
        public static Vector3Int up => new(0, 1, 0);
        public static Vector3Int down => new(0, -1, 0);
        public static Vector3Int left => new(-1, 0, 0);
        public static Vector3Int right => new(1, 0, 0);
        public static Vector3Int XZ => new(1, 0, 1);
        public static Vector3Int XY => new(1, 1, 0);
        public static Vector3Int YZ => new(0, 1, 1);
        public Vector3Int(int x = 0, int y = 0, int z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + x.GetHashCode();
                hash = hash * 23 + y.GetHashCode();
                hash = hash * 23 + z.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3Int))
                return false;

            var other = (Vector3Int)obj;
            return x == other.x && y == other.y && z == other.z;
        }

        public bool Equals(Vector3Int other)
        {
            return x == other.x && y == other.y && z == other.z;
        }
        public static Vector3Int operator +(Vector3Int left, Vector3Int b)
        {
            return new Vector3Int
            {
                x = left.x + b.x,
                y = left.y + b.y,
                z = left.z + b.z
            };
        }

        public static Vector3Int operator -(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int
            {
                x = a.x - b.x,
                y = a.y - b.y,
                z = a.z - b.z
            };
        }

        public static bool operator ==(Vector3Int a, Vector3Int b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector3Int a, Vector3Int b)
        {
            return !a.Equals(b);
        }
        public static Vector3Int operator *(Vector3Int pos, int multiple)
        {
            return new Vector3Int(pos.x * multiple, pos.y * multiple, pos.z * multiple);
        }
        public static Vector3Int FloorToVector3Int(Vector3 vect)
        {
            return new((int)vect.x, (int)vect.y, (int)vect.z);
        }
    }
}