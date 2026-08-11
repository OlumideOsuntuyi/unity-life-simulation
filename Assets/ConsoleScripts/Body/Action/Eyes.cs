using UnityEngine;

namespace Simulation
{
    public class Eyes
    {
        public Vector3 lookingAt { get; private set; }
        public bool foundTarget { get; private set; }
        public (SearchTarget.Type type, Animal life) Look(Animal self)
        {
            Ray ray = new Ray
            {
                origin = self.gameObject.transform.position,
                direction = self.gameObject.transform.Forward()
            };
            float sns = self.data.genes.modifications.sense * 10;
            if (Raycaster.Raycast(ray, out var hit, sns))
            {
                var life = hit.collider.gameObject.GetLife();
                if (life != null && life.UUID != self.UUID)
                {
                    if(life.data.genes.id == self.data.genes.id)
                    {
                        self.data.memory.FoundSameSpecies(self, life);
                        if(self.data.chromosome.type != life.data.chromosome.type)
                        {
                            Debug.Log($"seen mate");
                            return (SearchTarget.Type.Mate, life);
                        }
                    }
                    else
                    {
                        if(life.data.genes.modifications.power < self.data.genes.modifications.power)
                        {
                            self.data.memory.FoundFood(life.gameObject.transform.position);
                            return (SearchTarget.Type.Food, life);
                        }
                        else
                        {
                            self.data.memory.FoundEnemy(self, life);
                            return (SearchTarget.Type.Predator, life);
                        }
                    }
                    foundTarget = true;
                }
            }
            return (SearchTarget.Type.None, null);
        }
    }
}