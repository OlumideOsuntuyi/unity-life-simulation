using System.Collections.Generic;

using Simulation.Unity;

namespace Simulation
{
    [System.Serializable]
    public class SpeciesBank
    {
        private static SpeciesBank Instance;
        public static Dictionary<string, Species> species;
        public SpeciesBank(List<Species> s)
        {
            Instance = this;
            species = new();
            foreach(var gene in s)
            {
                species.Add(gene.id, gene);
            }
        }
        public SpeciesBank(List<SpeciesData> library)
        {
            Instance = this;
            species = new();
            foreach (var data in library)
            {
                Species specie = new Species(data.name, data.sourceGenes);
                species.Add(data.name, specie);
            }
        }

    }

    [System.Serializable]
    public class SpeciesLibrary
    {
        public static SpeciesLibrary Instance;
        public static string SavePath => "species.bin";
        public List<Simulation.Unity.SpeciesData> species = new();
        public SpeciesLibrary()
        {
            
        }
        public SpeciesLibrary(List<Unity.SpeciesData> data)
        {
            species = data;
        }
    }
}