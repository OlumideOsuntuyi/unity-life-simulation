using System;
using System.Collections.Generic;
using System.Numerics;

namespace Simulation
{
    public class Collider : Component
    {
        private float _radius, _friction, _bounciness;
        public Contacts inRange { get; private set; }
        public Contacts contacts { get; private set; }
        public float radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = Math.Abs(value);
            }
        }
        public float friction
        {
            get
            {
                return _friction;
            }
            set
            {
                _friction = Math.Clamp(value, 0, 1);
            }
        }
        public float bounciness
        {
            get
            {
                return _bounciness;
            }
            set
            {
                _bounciness = Math.Clamp(value, 0, 1);
            }
        }
        public Vector3 _size;
        public Vector3 size
        {
            get
            {
                return _size;
            }
        }
        public bool isTrigger;
        public override void Start()
        {
            _size = new(1, 1, 1);
            inRange = new();
            contacts = new();

            friction = 0.5f;
            bounciness = 0.25f;
        }
        public override void Update()
        {
            inRange = new Contacts() { contacts = new() };   
            List<Contact> contacts = new();
            foreach(SimObject obj in SimObject.objects)
            {
                if(obj != gameObject)
                {
                    if (IsInRange(obj.collider, out float distance))
                    {
                        inRange.contacts.Add(new Contact
                        {
                            distance = distance,
                            collider = obj.collider
                        });
                    }
                    contacts.AddRange(ContactPoints(obj.collider));
                }
            }
            this.contacts = new Contacts() { contacts = contacts };
        }
        public override void OnDestroy()
        {
            
        }
        public bool IsInRange(Collider col, out float distance)
        {
            if(col == this)
            {
                distance = 0;
                return false;
            }
            Vector3 halfSize = col.size * .5f;
            float closestX = Math.Clamp(transform.position.x, col.transform.position.x - halfSize.x, col.transform.position.x + halfSize.x);
            float closestY = Math.Clamp(transform.position.y, col.transform.position.y - halfSize.y, col.transform.position.y + halfSize.y);
            Vector3 closestPoint = new Vector3(closestX, closestY, transform.position.z);
            float range = Vector3.Distance(closestPoint, transform.position);
            if(range <= radius)
            {
                distance = range;
                return true;
            }
            distance = 0;
            return false;
        }
        public bool IsPointInCollider(Vector3 point)
        {
            if(size == new Vector3())
            {
                return false;
            }
            if (point.x > transform.position.x - size.x / 2 && point.x < transform.position.x + size.x / 2)
            {
                if (point.y > transform.position.y - size.y / 2 && point.y < transform.position.y + size.y / 2)
                {
                    if (point.z > transform.position.z - size.z / 2 && point.z < transform.position.z + size.z / 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsColliding(Collider col)
        {
            return CheckPointCollision(transform.position, size, col.transform.position, col.size);
        }
        public static bool CheckPointCollision(Vector3 positionA, Vector3 sizeA, Vector3 positionB, Vector3 sizeB)
        {
            // Calculate the extents of each collider (half the size)
            Vector3 extentsA = sizeA * 0.5f;
            Vector3 extentsB = sizeB * 0.5f;

            // Calculate the minimum and maximum points of each collider
            Vector3 minA = positionA - extentsA;
            Vector3 maxA = positionA + extentsA;
            Vector3 minB = positionB - extentsB;
            Vector3 maxB = positionB + extentsB;

            // Check for overlap along each axis
            bool overlapX = (maxA.x >= minB.x && minA.x <= maxB.x);
            bool overlapY = (maxA.y >= minB.y && minA.y <= maxB.y);
            bool overlapZ = (maxA.z >= minB.z && minA.z <= maxB.z);

            // If there is overlap along all axes, collision is detected
            return overlapX && overlapY && overlapZ;
        }
        public static void CalculatePositionInfo(Collider a, Collider b, out Vector3 normal, out Vector3 tangent)
        {
            normal = (a.transform.position - b.transform.position).normalized;
            tangent = Vector3.Cross(normal, Vector3.up);

        }
        public List<Contact> ContactPoints(Collider other)
        {
            List<Contact> contacts = new();
            Vector3 selfHalfExtents = size * .5f;
            Vector3 otherHalfExtents = other.size * .5f;
            Vector3[] thisAxes = GetAxes(transform.rotation);
            Vector3[] otherAxes = GetAxes(other.transform.rotation);

            Vector3 thisToOther = other.transform.position - transform.position;
            Vector3 otherToThis = -1 * thisToOther;

            foreach(Vector3 axis in thisAxes)
            {
                if(IsOverlapAxis(axis, selfHalfExtents, otherHalfExtents, thisToOther, otherAxes, out Vector3 normal))
                {
                    contacts.Add(new Contact
                    {
                        collider = other,
                        normal = normal,
                        tangent = Vector3.Cross(normal, Vector3.up)
                    });
                }
            }

            foreach (Vector3 axis in otherAxes)
            {
                if (IsOverlapAxis(axis, selfHalfExtents, otherHalfExtents, thisToOther, otherAxes, out Vector3 normal))
                {
                    contacts.Add(new Contact
                    {
                        collider = other,
                        normal = normal,
                        tangent = Vector3.Cross(normal, Vector3.up)
                    });
                }
            }
            if(contacts.Count > 0)
            {
                UnityEngine.Debug.Log("colliding");
            }
            return contacts;
        }
        private Vector3[] GetAxes(Vector3 rotation)
        {
            Quaternion quaternionRotation = Extensions.Euler(rotation);
            Vector3 xAxis = RotateVector(Vector3.right, quaternionRotation);
            Vector3 yAxis = RotateVector(Vector3.up, quaternionRotation);
            Vector3 zAxis = RotateVector(Vector3.forward, quaternionRotation);
            return new Vector3[]{ xAxis, yAxis, zAxis};
        }
        private Vector3 RotateVector(Vector3 vector3, Quaternion rotation)
        {
            Quaternion quaternionVector = new Quaternion(vector3.x, vector3.y, vector3.z, 0);
            quaternionVector = Quaternion.Normalize(rotation) * quaternionVector * Quaternion.Conjugate(rotation);
            return new Vector3(quaternionVector.X, quaternionVector.Y, quaternionVector.Z);
        }
        private bool IsOverlapAxis(Vector3 axis, Vector3 selfHalfExtents, Vector3 otherHalfExtents, Vector3 thisToOther, Vector3[] otherAxes, out Vector3 contactNormal)
        {
            float projectionThis = Math.Abs(Vector3.Dot(selfHalfExtents, axis));
            float projectionOther = Math.Abs(Vector3.Dot(otherHalfExtents, axis));
            float distance = Math.Abs(Vector3.Dot(thisToOther, axis));
            float overlap = projectionThis + projectionOther - distance;
            if(overlap > 0)
            {
                if(Vector3.Dot(thisToOther, axis) > 0)
                {
                    contactNormal = axis;
                }
                else
                {
                    contactNormal = -1 * axis;
                }
            }
            contactNormal = new();
            return false;
        }
        public Contacts CheckInRange()
        {
            List<Contact> objectsInRange = new();
            List<Contact> objectsInCollision = new();
            foreach (var obj in SimObject.objects)
            {
            }
            return new Contacts
            {
                contacts = objectsInRange
            };
        }

        [System.Serializable]
        public struct Contact
        {
            public float distance;
            public Vector3 normal, tangent;
            public Collider collider;
        }
        [System.Serializable]
        public struct Contacts
        {
            public List<Contact> contacts;
        }
    }
}