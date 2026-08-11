using System;

namespace Simulation
{
    [System.Serializable]
    public struct ChildData
    {
        public Genes genes;
        public Chromosome chromosome;
        public DateTime conception, natality;
        public bool inGestation;
        public ChildData(Animal mother, Animal father)
        {
            conception = DateTime.Now;
            chromosome = Chromosome.Combne(mother.data.chromosome, father.data.chromosome);
            genes = mother.data.genes.ProduceChild(father.data.genes);
            natality = conception.AddSeconds(0 * genes.GestationPeriod);
            inGestation = true;
        }
    }
}