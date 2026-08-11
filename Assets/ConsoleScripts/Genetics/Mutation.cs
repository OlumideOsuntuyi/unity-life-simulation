using System.Collections.Generic;

namespace Simulation
{
    public class MutationHandler
    {
        public Dictionary<string, GeneMutation> genes = new();

        public GenePotential Mutate(GenePotential gene, float mutationRange)
        {
            float chance = Math.Random(-mutationRange, mutationRange);
            List<string> properties = GenePotential.Properties();
            GenePotential mutatedGene = new() { name = gene.name };
            foreach (var prop in properties)
            {
                float value = gene.Get(prop) * (1 + chance);
                GenePotential.Set(prop, value, ref mutatedGene);
            }
            return mutatedGene;
        }
    }
    public class GeneMutation
    {
        public readonly GenePotential gene;
        public readonly float mutationRange;

        public GeneMutation(GenePotential gene, float mutationRange)
        {
            this.gene = gene;
            this.mutationRange = mutationRange;
        }
    }
}