using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Simulation
{
    public class SimObject
    {
        public static Dictionary<uint, SimObject> _objects = new();
        public static List<SimObject> objects = new();
        private static uint nextHash;
        private static readonly object _lock = new();
        public static void Update()
        {
            if(objects.Count > 0)
            {
                for (int index = 0; index < objects.Count; index++)
                {
                    if(index < objects.Count)
                    {
                        objects[index]?._Update();
                    }
                }
            }
        }
        public string name;
        public string UUID;
        public readonly uint hash;
        public Transform transform { get; private set; }
        public Collider collider { get; private set; }
        public DynamicBody rigidBody { get; private set; }

        private Dictionary<string, object> components = new();
        public int componentCount => components.Count;

        private bool _isActive = true;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
        }


        public SimObject()
        {
            name = "New GameObject";
            lock (_lock)
            {
                hash = nextHash;
                nextHash++;
            }
            Init();
        }
        public SimObject(string name)
        {
            this.name = name;
            lock (_lock)
            {
                hash = nextHash;
                nextHash++;
            }
            Init();
        }
        private void Init()
        {
            _isActive = true;
            components = new();
            transform = AddComponent<Transform>();
            collider = AddComponent<Collider>();
            rigidBody = AddComponent<DynamicBody>();

            lock (_lock)
            {
                _objects.Add(hash, this);
                objects.Add(this);
            }
        }
        private void _Update()
        {
            if(components != null)
            {
                var keys = components.Keys.ToList();
                if(keys.Count > 0)
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        ((IComponent)components[(string)keys[i]])?.Update();
                    }
                }
            }
            if(IsActive && false)
            {
                foreach (Transform transform in transform)
                {
                    transform.gameObject._Update();
                }
            }
        }
        public void Destroy()
        {
            _isActive = false;
            components.Clear();
            lock (_lock)
            {
                _objects.Remove(hash);
                objects.Remove(this);
            }
            UnityEngine.Debug.Log($"Destroy {UUID} object: hash:{hash}");
        }
        public Animal GetLife()
        {
            return GetComponent<Animal>();
        }
        public T GetComponent<T>() where T : Component
        {
            if (components.TryGetValue(typeof(T).Name, out var component))
            {
                return component as T;
            }
            return null;
        }
        public T AddComponent<T>() where T : IComponent, new()
        {
            var t = new T();
            ((IComponent)(t)).SetGameObject(hash);
            ((IComponent)(t)).Start();
            components.Add(typeof(T).Name, t);
            return t;
        }
    }
}