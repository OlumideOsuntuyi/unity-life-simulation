using System;
using System.Collections.Generic;
using System.Reflection;
namespace Simulation
{
    [System.Serializable]
    public struct GenePotential
    {
        public string name;
        //first order
        public float health, power, speed, sense, saturation, stamina, reproduction;
        //second order
        public float digestion, gestation, recovery, mutation, aggression;

        public float Weight()
        {
            var list = Properties();
            float total = 0f;
            foreach(var f in list)
            {
                total += Get(f);
            }
            return total / list.Count;
        }
        public static List<string> Properties()
        {
            List<string> names = new();
            Type type = typeof(GenePotential);
            FieldInfo[] fields = type.GetFields();
            foreach(var f in fields)
            {
                if(f.Name == "name")
                {
                    continue;
                }
                names.Add(f.Name);
            }
            return names;
        }
        public static void Set(string name, float value, ref GenePotential gene)
        {
            if (name.IsNullOrEmpty())
            {
                return;
            }
            value = Math.Clamp(value, 0, 99999);

            object obj = gene;
            Type type = typeof(GenePotential);
            FieldInfo fieldInfo = type.GetField(name);
            fieldInfo.SetValue(obj, value);
            gene = (GenePotential)obj;
        }
        public float Get(string name)
        {
            if(name.IsNullOrEmpty())
            {
                return 0;
            }
            Type type = typeof(GenePotential);
            FieldInfo fieldInfo = type.GetField(name);
            if (fieldInfo != null && fieldInfo.FieldType == typeof(float))
            {
                return (float)fieldInfo.GetValue(this);
            }
            else
            {
                throw new ArgumentException("Field not found or not of type float");
            }
        }
        public static GenePotential operator+(GenePotential a, GenePotential b)
        {
            List<string> properties = Properties();
            GenePotential result = new();
            foreach(var mod in properties)
            {
                Set(mod, a.Get(mod) + b.Get(mod), ref result);
            }
            return result;
        }
        public static GenePotential operator *(GenePotential a, float b)
        {
            List<string> properties = Properties();
            GenePotential result = new();
            foreach (var mod in properties)
            {
                Set(mod, a.Get(mod) * b, ref result);
            }
            return result;
        }
        public override string ToString()
        {
            var props = Properties();
            string log = $"{name} stats: ";
            foreach (var g in props)
            {
                if (Get(g) > 0)
                {
                    log += $"{g}: {Get(g)}, ";
                }
            }
            return log;
        }
    }

    [System.Serializable]
    public struct SpeciePotential
    {
        public List<GenePotential> genes;
    }
}