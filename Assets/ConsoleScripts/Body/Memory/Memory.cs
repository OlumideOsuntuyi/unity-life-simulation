using System.Collections.Generic;

namespace Simulation
{
    [System.Serializable]
    public class Memory
    {
        public Dictionary<MemoryType, List<MemorySlot>> memories;
        public Relationships relationship { get; private set; }
        public Memory()
        {
            memories = new();
            relationship = new();
        }
        public void Update(Animal life)
        {

        }
        public void AddMemory(MemoryType type, object memory)
        {
            if(memories.ContainsKey(type))
            {
                memories[type].Add(new MemorySlot { memory = memory, type = type });
            }
            else
            {
                memories.Add(type, new List<MemorySlot> { new MemorySlot { memory = memory, type = type } });
            }
        }
        public void FoundFood(Vector3 point)
        {
            AddMemory(MemoryType.FoundFood, point);
        }
        public void FoundEnemy(Animal self, Animal other)
        {
            relationship.AddRelation(other, Relationship.Type.Enemy);
        }
        public void FoundSameSpecies(Animal self, Animal other)
        {
            if(self.data.chromosome.type != other.data.chromosome.type)
            {
                AddMemory(MemoryType.FoundMate, other.gameObject.transform.position);
            }
        }
        public bool FindFood(Animal self, out Vector3 position)
        {
            if (memories.TryGetValue(MemoryType.FoundFood, out var locations))
            {
                float minDis = float.PositiveInfinity;
                Vector3 candidate = new();
                foreach (var location in locations)
                {
                    var pos = (Vector3)location.memory;
                    float dis = Vector3.Distance(self.gameObject.transform.position, pos);
                    if(dis < minDis)
                    {
                        candidate = pos;
                        minDis = dis;
                    }
                }
                if(minDis < float.PositiveInfinity)
                {
                    position = candidate;
                    locations.Remove(locations.Find(l => l.memory == (object)candidate));
                    return true;
                }
            }
            position = self.gameObject.transform.position;
            return false;
        }
    }
    [System.Serializable]
    public struct MemorySlot
    {
        public object memory;
        public MemoryType type;
    }
    public enum MemoryType { FoundMate, FoundFood}
}