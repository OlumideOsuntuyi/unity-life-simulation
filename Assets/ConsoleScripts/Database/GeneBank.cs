using System.Collections.Generic;

namespace Simulation
{
    [System.Serializable]
    public class GeneBank
    {
        public static GeneBank Instance;
        public List<GenePotential> genes = new();
        public List<GenePotential> mutated_genes = new();

        public GenePotential Get(string id)
        {
            return genes.Find(g => g.name == id);
        }
    }
}