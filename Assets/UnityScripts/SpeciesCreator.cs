using System.Collections.Generic;
using System.IO;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Simulation.Unity
{
    public class SpeciesCreator : MonoBehaviour
    {
        public static SpeciesCreator Instance;
        public SpeciesLibrary library;
        public GenePotential multipliers;
        public Benchmarks benchmark;
        public List<Model> models;
        private void Awake()
        {
            Instance = this;
            editorSpecies = new();
            benchmark = new(multipliers);
            library = FileHandler.LoadObject<SpeciesLibrary>(Path.Combine(Application.persistentDataPath, SpeciesLibrary.SavePath), false);
            SpeciesLibrary.Instance = library;
        }
        private void OnDestroy()
        {
            FileHandler.SaveObject(library, Path.Combine(Application.persistentDataPath, SpeciesLibrary.SavePath), false);
        }
        private void Start()
        {
            creator.Reset();
        }
        private void Update()
        {
            creator.Update();
            benchmark.benchmark = multipliers;
        }
        public List<Simulation.Species> GetSimulationSpecies()
        {
            SpeciesBank bank = new SpeciesBank(library.species);
            List<Simulation.Species> species = new();
            foreach(var es in SpeciesBank.species)
            {
                species.Add(es.Value);
            }
            return species;
        }
        public LifeModel GetModel(int index)
        {
            return models[index].model;
        }
        public void Clear()
        {
            UnityShortcuts.ClearTransform(UI.content);
            editorSpecies = new();
        }
        [System.Serializable]
        public struct Model
        {
            public string name;
            public LifeModel model;
        }
        [System.Serializable]
        public class UIElements
        {
            public RectTransform content;
            public SpeciesInfo speciesPrefab;
            public TMP_Text specieCount;
            public SimMono lifePrefab;
        }
        public UIElements UI;
        public List<SpeciesInfo> editorSpecies;
        public void CreateSpecies()
        {
            var clone = Instantiate(UI.speciesPrefab, UI.content);
            editorSpecies.Add(clone);
            clone.ListSourceGenes();
        }

        public Creator creator;
        public void NewSpecies()
        {
            creator.Reset();
        }
        public void Creator_AddGene()
        {
            if(creator.geneDropDown.options.Count > 0)
            {
                creator.AddGene();
            }
        }
        public void SaveSpecies()
        {
            if(!creator.nameInput.text.IsNullOrEmpty())
            {
                library.species.Add(creator.GetSpecies());
                creator.Reset();
            }
        }
        [System.Serializable]
        public class Creator
        {
            public TMP_InputField nameInput;
            public TMP_InputField descriptionInput;

            public TMP_Text pointsLeftText;
            public TMP_Dropdown geneDropDown;

            public Image addGeneGraphic;

            //contents
            public RectTransform genesContent;
            public RectTransform statContent;

            //prefabs
            public SpeciesStat statPrefab;
            public SpeciesGene genePrefab;

            public int addGeneDefault, addGeneDisabled;
            public Color[] colors;
            public List<string> genes = new();
            public List<float> gene_weight = new();
            public float maxEnergyPoints;
            public float pointsConsumed = 0;
            public float pointsLeft
            {
                get
                {
                    return maxEnergyPoints - pointsConsumed;
                }
            }
            public float PointsConsumed()
            {
                float cost = 0;
                foreach (var gene in genes)
                {
                    GenePotential g = GeneMenu.Instance.geneBank.genes.Find(g => g.name == gene);
                    cost += g.Weight();
                }
                return cost;
            }

            public GenePotential GenePotential
            {
                get
                {
                    GenePotential gp = new GenePotential();
                    int step = 0;
                    foreach (var gene in genes)
                    {
                        GenePotential g = GeneMenu.Instance.geneBank.genes.Find(g => g.name == gene) * gene_weight[step];
                        gp += g;
                        step++;
                    }
                    return gp;
                }
            }

            public void Reset()
            {
                nameInput.text = "";
                descriptionInput.text = "";
                StaticShortcuts.ClearChildren(genesContent);
                StaticShortcuts.ClearChildren(statContent);
                List<string> props = GenePotential.Properties();
                foreach(var s in props)
                {
                    var clone = Instantiate(statPrefab, statContent);
                    clone.Set(s);
                }
                ListGenes();
            }
            public void Update()
            {
                pointsLeftText.text = $"{pointsLeft:F1}";
                bool canAdd = pointsLeft > 0 && geneDropDown.options.Count > 0;
                addGeneGraphic.color = colors[canAdd ? addGeneDefault : addGeneDisabled];
                addGeneGraphic.raycastTarget = canAdd;
            }
            public void AddGene()
            {
                var gene = GeneMenu.Instance.geneBank.genes.Find(g => g.name == geneDropDown.options[geneDropDown.value].text);
                if(gene.Weight() < pointsLeft && !genes.Contains(gene.name))
                {
                    genes.Add(gene.name);
                    gene_weight.Add(1f);
                    ListGenes();
                }
            }
            public void RemoveGene(string gene)
            {
                int index = genes.IndexOf(gene);
                genes.Remove(gene);
                gene_weight.RemoveAt(index);
                ListGenes();
            }
            public void ListGenes()
            {
                List<string> geneNames = SortGenesByTag(new List<string>());
                foreach(var g in genes)
                {
                    geneNames.Remove(g);
                }
                geneDropDown.ClearOptions();
                geneDropDown.AddOptions(geneNames);
                pointsConsumed = PointsConsumed();

                ListAddedGenes();
            }
            public void ListAddedGenes()
            {
                StaticShortcuts.ClearChildren(genesContent);
                foreach(var gene in genes)
                {
                    var clone = Instantiate(genePrefab, genesContent);
                    clone.Set(gene);
                }
            }
            public List<string> SortGenesByTag(List<string> tags)
            {
                List<string> result = new();
                foreach(var g in GeneMenu.Instance.geneBank.genes)
                {
                    bool found = false;
                    foreach(var tag in tags)
                    {
                        if(g.Get(tag) > 0)
                        {
                            found = true;
                            break;
                        }
                    }
                    if(tags.Count == 0 || found)
                    {
                        result.Add(g.name);
                    }
                }
                return result;
            }
            public SpeciesData GetSpecies()
            {
                List<GenePotential> genes = new();
                int step = 0;
                foreach(var gene in this.genes)
                {
                    genes.Add(GeneMenu.Instance.geneBank.genes.Find(g => g.name == gene) * gene_weight[step]);
                    step++;
                }
                SpeciesData data = new SpeciesData(nameInput.text, genes);

                return data;
            }
        }
    }
}