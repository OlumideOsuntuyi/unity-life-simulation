using System.Collections.Generic;

namespace Simulation
{
    [System.Serializable]
    public struct Relationship
    {
        public string UUID;
        public Type type;
        public enum Type { Friend, Enemy, Family, Neutral }
    }

    [System.Serializable]
    public class Relationships
    {
        public Dictionary<string, Relationship> relationships = new();
        public int memories => relationships.Count;
        public List<Relationship> relationshipsList
        {
            get
            {
                List<Relationship> list = new List<Relationship>();
                foreach(var kvp in relationships)
                {
                    list.Add(kvp.Value);
                }
                return list;
            }
        }
        public void AddRelation(Animal life, Relationship.Type type)
        {
            if(GetRelation(life.UUID) == Relationship.Type.Neutral)
            {

            }
            else
            {
                relationships.Remove(life.UUID);
            }
            if(type is not Relationship.Type.Neutral)
            {
                relationships.Add(life.UUID, new Relationship() { UUID = life.UUID, type = type });
            }
        }
        public Relationship.Type GetRelation(string UUID)
        {
            if (relationships.ContainsKey(UUID))
            {
                return relationships[UUID].type;
            }
            return Relationship.Type.Neutral;
        }
    }
}