using System.Collections.Generic;

using Simulation.Unity;

namespace Simulation
{
    [System.Serializable]
    public class Genes
    {
        public string id;
        public readonly int generation;
        public Dictionary<string, Gene> genes = new();
        public GenePotential modifications;
        public GeneticReward reward;

        const float BIRTH_CONSTANT = 30f;
        const float DEATH_CONSTANT = 1f;

        private List<Gene> list = new();   
        public float GestationPeriod
        {
            get
            {
                float value = modifications.gestation;
                return BIRTH_CONSTANT * Math.Max(0.1f, value);
            }
        }
        public float EvolutionEnergy
        {
            get
            {
                float value = 0;
                foreach(var gene in list)
                {
                    value += gene.Weight;
                }
                return value;
            }
        }
        public Genes(string id, List<Gene> genes, int generation = 1)
        {
            this.id = id;
            this.generation = generation;
            list = genes;

            modifications = new();
            reward = new();
            for (int i = 0; i < genes.Count; i++)
            {
                this.genes.Add(genes[i].ID, genes[i]);
                modifications += genes[i].Modifications;
            }
        }
        public Genes ProduceChild(Genes father)
        {
            float gestationEnergy = (father.modifications.gestation + modifications.gestation) + (father.modifications.mutation + modifications.mutation);
            gestationEnergy *= 2f;
            List<Gene> genes = new();

            foreach(var gene in father.genes)
            {
                if(this.genes.ContainsKey(gene.Key))
                {
                    genes.Add(Propagate(this.genes[gene.Key], gene.Value, father.reward, reward, ref gestationEnergy));
                }
                else
                {
                    if(gene.Value.Type.Reduce().alleles is not Alleles.Extinct)
                    {
                        genes.Add(gene.Value);
                    }
                }
            }
            foreach(var gene in this.genes)
            {
                if(genes.Find(g => g.ID == gene.Value.ID).ID.IsNullOrEmpty())
                {
                    if (gene.Value.Type.Reduce().alleles is not Alleles.Extinct)
                    {
                        genes.Add(gene.Value);
                    }
                }
            }
            Genes child = new Genes(father.id, genes, (int)Math.Max(generation, father.generation) + 1);

            return child;
        }
        public Gene Propagate(Gene motherGene, Gene fatherGene, GeneticReward fatherReward, GeneticReward motherReward, ref float gestationEnergy)
        {
            GeneStrength strength = GeneStrength.CombineType(motherGene.Type, fatherGene.Type);
            List<string> properties = GenePotential.Properties();

            float totalWeight = fatherGene.Weight + motherGene.Weight;

            float fatherSccore = fatherGene.Weight / totalWeight;
            float motherScore = motherGene.Weight / totalWeight;

            float value = (fatherGene.Value * fatherSccore) + (motherGene.Value * motherScore);

            float totalRwd = 0;

            foreach (var prop in properties)
            {
                float fatherRwd = fatherReward.GetScore(prop);
                float motherRwd = motherReward.GetScore(prop);
                totalRwd += fatherRwd + motherRwd;
            }
            if(totalRwd < 0)
            {
                strength = strength.Reduce();
                if (gestationEnergy > 0)
                {
                    float mutation = Math.Random(0, .2f);
                    if (gestationEnergy >= mutation)
                    {
                        gestationEnergy -= mutation;
                        value *= 1 - mutation;
                    }
                }
            }
            else if(totalRwd > 0)
            {
                strength = strength.Increase();
                if (gestationEnergy > 0)
                {
                    float mutation = Math.Random(0, .2f);
                    if(gestationEnergy >= mutation)
                    {
                        gestationEnergy -= mutation;
                        value *= 1 + mutation;
                    }
                }
            }
            string ID = motherGene.Weight > fatherGene.Weight ? motherGene.ID : fatherGene.ID;

            return new Gene(ID, value, strength);
        }
        public bool Get(string id, out Gene gene)
        {
            if (genes.TryGetValue(id, out var result))
            {
                gene = result;
                return true;
            }
            gene = default;
            return false;
        }
        public Gene Get(string id)
        {
            if(genes.TryGetValue(id, out var result))
            {
                return result;
            }
            return default;
        }
    }
    [System.Serializable]
    public struct Gene
    {
        public string ID;
        public readonly float Value;
        public readonly float Weight;
        public readonly GeneStrength Type;
        public readonly GenePotential Modifications;
        public Gene(string id, float value, GeneStrength type)
        {
            this.ID = id;
            this.Type = type;
            this.Value = value;
            Modifications = GeneBank.Instance.Get(id) * value;
            Weight = Modifications.Weight();
        }
    }

    public class GeneticReward
    {
        readonly Dictionary<string, float> scores;
        public GeneticReward()
        {
            scores = new();
            List<string> props = GenePotential.Properties();
            foreach(var prop in props)
            {
                scores.Add(prop.Trim().ToLower(), 0f);
            }
        }
        public float GetScore(string name)
        {
            if(scores.TryGetValue(name.Trim().ToLower(), out var result))
            {
                return result;
            }
            return 0f;
        }
        public bool SetScore(string name, float value)
        {
            if(scores.ContainsKey(name.Trim().ToLower()))
            {
                scores[name.Trim().ToLower()] = value;
                return true;
            }
            return false;
        }
    }
}