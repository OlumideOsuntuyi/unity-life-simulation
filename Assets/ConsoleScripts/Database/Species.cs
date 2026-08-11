using System.Collections.Generic;

namespace Simulation
{
    [System.Serializable]
    public class Species
    {
        public string id;
        public readonly float totalWeight;
        public Dictionary<string, GenePotential> genes;
        public Species(string id, List<GenePotential> genes)
        {
            this.id = id;
            this.genes = new();
            foreach(var gene in genes)
            {
                totalWeight += gene.Weight();
                this.genes.Add(gene.name, gene);
            }
        }
        public Genes FirstGeneration()
        {
            List<Gene> list = new();
            foreach (var gene in this.genes)
            {
                list.Add(new Gene(gene.Key, 1 + Math.Random(-0.2f, 0.2f), new GeneStrength((Alleles)Math.Random(0, 2))));
            }
            Genes genes = new(id, list);
            return genes;
        }

    }
}