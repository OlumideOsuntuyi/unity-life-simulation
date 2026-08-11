using System.Collections.Generic;

namespace Simulation.Unity
{

    [System.Serializable]
    public class SpeciesData
    {
        public string name;
        public List<GenePotential> sourceGenes;
        public SpeciesData()
        {
            name = "";
            sourceGenes = new();
        }
        public SpeciesData(string name, List<GenePotential> sourceGenes)
        {
            this.name = name;
            this.sourceGenes = sourceGenes;
        }
    }
}