using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Simulation
{
    public class Transform : Component, IEnumerable<Transform>
    {
        private Transform _parent;
        private List<Transform> children;
        private Vector3 _position, _localPosition;
        private Vector3 _rotation, _localRotation;


        public Vector3 position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value != _position)
                {
                    _position = value;
                    if (_parent != null)
                    {
                        _localPosition = _position - _parent._position;
                    }
                    else
                    {
                        _localPosition = _position;
                    }
                }
            }
        }
        public Vector3 localPosition
        {
            get
            {
                return localPosition;
            }
            set
            {
                if (value != _localPosition || _parent == null)
                {
                    _localPosition = value;
                    if (_parent == null)
                    {
                        _position = _localPosition;
                    }
                    else
                    {
                        _position = _localPosition + _parent._position;
                    }
                }
            }
        }
        public Vector3 rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                if (value != _rotation)
                {
                    _rotation = value;
                    if (_parent != null)
                    {
                        _localRotation = _rotation - _parent._rotation;
                    }
                    else
                    {
                        _localRotation = _rotation;
                    }
                }
            }
        }
        public Vector3 localRotation
        {
            get
            {
                return localRotation;
            }
            set
            {
                if (value != _localRotation || _parent == null)
                {
                    _localRotation = value;
                    if (_parent == null)
                    {
                        _rotation = _localRotation;
                    }
                    else
                    {
                        _rotation = _localRotation + _parent._rotation;
                    }
                }
            }
        }


        public Transform Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (value != this)
                {
                    _parent = value;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<Transform> GetEnumerator()
        {
            return children.GetEnumerator();
        }
        public override void Start()
        {
            children = new();
            position = Vector3.Rand(-20, 20, new Vector3(1, 0, 1));
            rotation = Vector3.Rand(-180, 180, new Vector3(0, 1, 0));
        }
        public void SetParent(Transform parent)
        {
            if (parent == this)
            {
                return;
            }
            if (parent == null && _parent != null)
            {
                RemoveParent();
            }
            else if (parent != null && _parent != null)
            {
                _parent = parent;
                parent.AddChild(this);
                SetLocal();
            }
        }
        private void RemoveParent()
        {
            _parent = null;
            _parent.RemoveChild(this);
            SetLocal();
        }
        private void SetLocal()
        {
            Transform value = _parent;
            if (value == null && _parent != null)
            {
                _localPosition = position;
                _localRotation = rotation;
            }
            else if (value != null && _parent != null)
            {
                _localPosition = _position - value._position;
                _localRotation = _rotation - value._rotation;
            }
        }
        private void AddChild(Transform child)
        {
            if (child == this)
            {
                return;
            }
            children.Add(child);
        }
        private void RemoveChild(Transform child)
        {
            children.Remove(child);
        }
        public void Rotate(Vector3 eulerAngles)
        {
            rotation += eulerAngles;
        }

        public void Translate(Vector3 translation)
        {
            position += translation;
        }
        public void LookAt(Vector3 target, float weight = 1)
        {
            rotation = Vector3.Lerp(rotation, RotateTowards(position, target), weight);
        }
        public static Vector3 LookRotation(Vector3 forward, Vector3 upwards)
        {
            float yaw = System.MathF.Atan2(forward.z, forward.x);
            float pitch = System.MathF.Atan2(forward.y, Math.Sqrt(forward.x * forward.x + forward.z * forward.z));

            float roll = 0f;

            return new Vector3(pitch, yaw, roll);
        }
        public static Vector3 RotateTowards(Vector3 position, Vector3 targetPosition)
        {
            Vector3 directionToTarget = new Vector3(
                targetPosition.x - position.x,
                targetPosition.y - position.y,
                targetPosition.z - position.z
            );

            Vector3 resultingRotation = LookRotation(directionToTarget, Vector3.up);

            return resultingRotation;
        }
        public Vector3 InverseTransformPoint(Vector3 worldPoint)
        {
            if (_parent != null)
            {
                return worldPoint - _parent.position;
            }
            else
            {
                return worldPoint;
            }
        }

        public Vector3 Direction(Vector3 direction)
        {
            Vector3 forward = Forward();
            Vector3 right = Right();
            Vector3 up = Up();

            return direction.x * right + direction.y * up + direction.z * forward;
        }


        public Vector3 Forward()
        {
            return RotateVector(Vector3.forward, rotation);
        }

        public Vector3 Up()
        {
            return RotateVector(Vector3.up, rotation);
        }

        public Vector3 Right()
        {
            return RotateVector(Vector3.right, rotation);
        }

        private static Vector3 RotateVector(Vector3 vector, Vector3 eulerAngles)
        {
            float radX = (float)(System.Math.PI / 180) * eulerAngles.x;
            float radY = (float)(System.Math.PI / 180) * eulerAngles.y;
            float radZ = (float)(System.Math.PI / 180) * eulerAngles.z;

            float sinX = (float)System.Math.Sin(radX);
            float cosX = (float)System.Math.Cos(radX);
            float sinY = (float)System.Math.Sin(radY);
            float cosY = (float)System.Math.Cos(radY);
            float sinZ = (float)System.Math.Sin(radZ);
            float cosZ = (float)System.Math.Cos(radZ);

            float newX = vector.x * (cosY * cosZ) + vector.y * (sinX * sinY * cosZ - cosX * sinZ) + vector.z * (cosX * sinY * cosZ + sinX * sinZ);
            float newY = vector.x * (cosY * sinZ) + vector.y * (sinX * sinY * sinZ + cosX * cosZ) + vector.z * (cosX * sinY * sinZ - sinX * cosZ);
            float newZ = vector.x * (-sinY) + vector.y * (sinX * cosY) + vector.z * (cosX * cosY);

            return new Vector3(newX, newY, newZ);
        }
        public void Rotate(Vector3 axis, float angle)
        {
            float radians = angle * (float)System.Math.PI / 180f;

            float cosAngle = (float)System.Math.Cos(radians);
            float sinAngle = (float)System.Math.Sin(radians);

            float crossX = rotation.y * axis.z - rotation.z * axis.y;
            float crossY = rotation.z * axis.x - rotation.x * axis.z;
            float crossZ = rotation.x * axis.y - rotation.y * axis.x;

            float dotProduct = rotation.x * axis.x + rotation.y * axis.y + rotation.z * axis.z;

            // Apply the Rodrigues' rotation formula
            float newX = rotation.x * cosAngle + crossX * sinAngle + axis.x * dotProduct * (1 - cosAngle);
            float newY = rotation.y * cosAngle + crossY * sinAngle + axis.y * dotProduct * (1 - cosAngle);
            float newZ = rotation.z * cosAngle + crossZ * sinAngle + axis.z * dotProduct * (1 - cosAngle);

            // Update the rotation components
            rotation = new(newX, newY, newZ);
        }

        public static Vector3 ToEulerAngles(Quaternion quaternion)
        {
            // Extract individual components of the quaternion
            float x = quaternion.X;
            float y = quaternion.Y;
            float z = quaternion.Z;
            float w = quaternion.W;

            // Calculate Euler angles
            float pitch = (float)System.MathF.Atan2(2 * (y * z + w * x), w * w - x * x - y * y + z * z);
            float yaw = (float)System.MathF.Asin(-2 * (x * z - w * y));
            float roll = (float)System.MathF.Atan2(2 * (x * y + w * z), w * w + x * x - y * y - z * z);

            // Convert to degrees if needed
            pitch = Math.ToDegrees(pitch);
            yaw = Math.ToDegrees(yaw);
            roll = Math.ToDegrees(roll);

            return new Vector3(pitch, yaw, roll);
        }

        public static Quaternion FromEulerAngles(Vector3 eulerAngles)
        {
            float pitch = Math.Deg2Rad * eulerAngles.z;
            float yaw = Math.Deg2Rad * eulerAngles.y;
            float roll = Math.Deg2Rad * eulerAngles.z;

            float halfPitch = pitch * 0.5f;
            float halfYaw = yaw * 0.5f;
            float halfRoll = roll * 0.5f;

            float sinPitch = Math.Sin(halfPitch);
            float cosPitch = Math.Cos(halfPitch);
            float sinYaw = Math.Sin(halfYaw);
            float cosYaw = Math.Cos(halfYaw);
            float sinRoll = Math.Sin(halfRoll);
            float cosRoll = Math.Cos(halfRoll);

            float x = sinRoll * cosPitch * cosYaw + cosRoll * sinPitch * sinYaw;
            float y = cosRoll * sinPitch * cosYaw - sinRoll * cosPitch * sinYaw;
            float z = cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw;
            float w = cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw;

            return new Quaternion(x, y, z, w);
        }
    }
}