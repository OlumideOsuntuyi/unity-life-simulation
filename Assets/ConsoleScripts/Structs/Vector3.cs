using System;

using UnityEngine;

namespace Simulation
{
    [System.Serializable]
    public struct Vector3 : IEquatable<Vector3>
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float x = 0, float y = 0, float z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static Vector3 zero => new(0, 0, 0);
        public static Vector3 one => new(1, 1, 1);
        public static Vector3 forward => new(0, 0, 1);
        public static Vector3 back => new(0, 0, -1);
        public static Vector3 up => new(0, 1, 0);
        public static Vector3 down => new(0, -1, 0);
        public static Vector3 left => new(-1, 0, 0);
        public static Vector3 right => new(1, 0, 0);
        public static Vector3 XZ => new(1, 0, 1);
        public static Vector3 XY => new(1, 1, 0);
        public static Vector3 YZ => new(0, 1, 1);
        public static Vector3 Rand(float min, float max, Vector3 axis, float absMin = 0)
        {
            absMin = Mathf.Clamp(Mathf.Abs(absMin), min, max);
            float[] values = new float[3] { 0, 0, 0 };
            for (int i = 0; i < 3; i++)
            {
                float value = Math.Random(min, max);
                while(Math.Abs(value) < absMin)
                {
                    value = Math.Random(min, max);
                }
                values[i] = value;
            }
            return new(values[0] * axis.x, values[1] * axis.y, values[2] * axis.z);
        }
        public Vector3 normalized
        {
            get
            {
                if(x == y && y == z && z == 0)
                {
                    return zero;
                }
                float m = Magnitude(this);
                return new Vector3(x / m, y / m, z / m);
            }
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
            if (!(obj is Vector3))
                return false;

            var other = (Vector3)obj;
            return x == other.x && y == other.y && z == other.z;
        }

        public bool Equals(Vector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }
        public override string ToString()
        {
            return $"({x}:{y}:{z})";
        }
        public static Vector3 operator +(Vector3 left, Vector3 b)
        {
            return new Vector3
            {
                x = left.x + b.x,
                y = left.y + b.y,
                z = left.z + b.z
            };
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3
            {
                x = a.x - b.x,
                y = a.y - b.y,
                z = a.z - b.z
            };
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !a.Equals(b);
        }
        public static Vector3 operator +(Vector3 a, Vector3Int b)
        {
            return new Vector3
            {
                x = a.x + (int)b.x,
                y = a.y + (int)b.y,
                z = a.z + (int)b.z
            };
        }

        public static Vector3 operator -(Vector3 a, Vector3Int b)
        {
            return new Vector3
            {
                x = a.x - b.x,
                y = a.y - b.y,
                z = a.z - b.z
            };
        }
        public static Vector3 operator *(Vector3 vector, float value)
        {
            return new Vector3(vector.x * value, vector.y * value, vector.z * value);
        }
        public static Vector3 operator *(float value, Vector3 vector)
        {
            return vector * value;
        }
        public static Vector3 operator /(Vector3 vector, float value)
        {
            return new Vector3(vector.x / value, vector.y / value, vector.z / value);
        }
        public static Vector3 operator /(float value, Vector3 vector)
        {
            return vector / value;
        }
        public static implicit operator Vector3(UnityEngine.Vector3 value)
        {
            return new Vector3(value.x, value.y, value.z);
        }

        public static implicit operator UnityEngine.Vector3(Vector3 value)
        {
            return new(value.x, value.y, value.z);
        }
        public static float Distance(Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;

            float dxSquared = dx * dx;
            float dySquared = dy * dy;
            float dzSquared = dz * dz;

            float sumOfSquares = dxSquared + dySquared + dzSquared;

            float distance = (float)System.Math.Sqrt(sumOfSquares);

            return distance;
        }
        public static float Dot(Vector3 a, Vector3 b)
        {
            return UnityEngine.Vector3.Dot(a, b);
        }
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return UnityEngine.Vector3.Cross(a, b);
        }
        public static float Magnitude(Vector3 v)
        {
            return (float)System.Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        }
        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            float sqrMagnitude = vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
            if (sqrMagnitude > maxLength * maxLength)
            {
                float magnitude = Mathf.Sqrt(sqrMagnitude);
                float ratio = maxLength / magnitude;
                vector.x *= ratio;
                vector.y *= ratio;
                vector.z *= ratio;
            }
            return vector;
        }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );
        }

        public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            float angle = Angle(a, b);
            return Lerp(a, b, angle / (float)System.Math.PI * t);
        }

        public static float Angle(Vector3 a, Vector3 b)
        {
            float dot = Dot(a.normalized, b.normalized);
            return (float)System.Math.Acos(Math.Clamp(dot, -1f, 1f));
        }
    }
}