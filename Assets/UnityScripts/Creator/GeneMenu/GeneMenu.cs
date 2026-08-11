using System.Collections.Generic;
using System.IO;
using System.Linq;

using TMPro;

using UnityEngine;

namespace Simulation.Unity
{
    public class GeneMenu : Singleton<GeneMenu>
    {
        [System.Serializable]
        public class UIElements
        {
            public TMP_InputField geneNameInput;
            public TMP_Dropdown modList;
            public RectTransform geneContent;
            public GeneData genePrefab;

            public TMP_Text energyConsumed;

            public GeneMod modPrefab;
            public RectTransform modContent;
            public List<FieldReMod> fieldMods = new();
        }

        public UIElements UI;
        public GeneBank geneBank;
        public GenePotential editing;
        private List<string> addedGeneModifications;
        private bool isEditing;
        private string GENE_SAVEPATH => Path.Combine(Application.persistentDataPath, "Genes.bin");
        private void Awake()
        {
            geneBank = FileHandler.LoadObject<GeneBank>(GENE_SAVEPATH, false);
            GeneBank.Instance = geneBank;
        }
        private void OnDestroy()
        {
            FileHandler.SaveObject(geneBank, GENE_SAVEPATH, false);
        }
        private void Start()
        {
            CreateNewGene();
        }
        private void Update()
        {
            UI.energyConsumed.text = $"Evolution energy left: {(EnergyLeftForEdit() * 10f):F2}";
        }
        private float EnergyConsumedInEdit()
        {
            return editing.Weight();
        }
        public const float TOTAL_GENE_ENERGY = 2f;
        public float EnergyLeftForEdit()
        {
            return TOTAL_GENE_ENERGY - EnergyConsumedInEdit();
        }
        public void ListAvaliableGenes()
        {
            StaticShortcuts.ClearChildren(UI.geneContent);
            foreach(var gene in geneBank.genes)
            {
                var clone = Instantiate(UI.genePrefab, UI.geneContent);
                clone.Set(gene);
            }
        }
        public void CreateNewGene()
        {
            editing = new();
            UI.geneNameInput.text = "";
            addedGeneModifications = new();
            ListAvaliableGenes();
            ListAddedGeneModifications();
        }
        public void OnGeneNameChange()
        {
            editing.name = UI.geneNameInput.text;
        }
        public void AddMod()
        {
            addedGeneModifications.Add(UI.modList.options[UI.modList.value].text);
            ListAddedGeneModifications();
        }
        public void RemoveMod(GeneMod mod)
        {
            GenePotential.Set(mod.title, 0, ref editing);
            addedGeneModifications.Remove(mod.title);
            ListAddedGeneModifications();
        }
        public void SetDropDowns()
        {
            List<string> modsLeft = GenePotential.Properties();
            foreach (var m in addedGeneModifications)
            {
                modsLeft.Remove(m);
            }
            UI.modList.ClearOptions();
            UI.modList.AddOptions(modsLeft);
        }
        public void ListAddedGeneModifications()
        {
            SetDropDowns();
            StaticShortcuts.ClearChildren(UI.modContent);
            foreach(string field in addedGeneModifications)
            {
                var clone = Instantiate(UI.modPrefab, UI.modContent);
                float value = editing.Get(field);
                clone.Set(field, value, UI.fieldMods.Find(f => f.name.Trim().ToLower() == field.Trim().ToLower()));
            }
        }
        public void EditGene(int index)
        {
            ListAddedGeneModifications();
            GenePotential gene = geneBank.genes[index];
            var mods = UI.modContent.GetComponentsInChildren<GeneMod>();
            foreach(var mod in mods)
            {
                mod.selector.value = Mathf.InverseLerp(GeneMod.min_, GeneMod.max_, gene.Get(mod.title));
            }
        }
        public void EditMod(GeneMod mod)
        {
            GenePotential.Set(mod.title, mod.value, ref editing);
        }
        public void SaveEdited()
        {
            SaveGene(editing);
        }
        private bool SaveGene(GenePotential potential)
        {
            if (!potential.name.IsNullOrEmpty())
            {
                var f = geneBank.genes.Find(g => g.name == potential.name);
                if(!f.name.IsNullOrEmpty())
                {
                    geneBank.genes.Remove(f);
                }
                geneBank.genes.Add(potential);
                CreateNewGene();
                return true;
            }
            return false;
        }

        [System.Serializable]
        public struct FieldReMod
        {
            public string name;
            public string newName;
            public string units;
            public Color color;
            public string hex => ColorUtility.ToHtmlStringRGBA(color);
        }
    }
}