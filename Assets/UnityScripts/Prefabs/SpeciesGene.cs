using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Simulation.Unity
{
    public class SpeciesGene : MonoBehaviour
    {
        public TMP_Text label;
        public Slider slider;
        private string gene;
        float prevValue;
        public void OnChangeSlider(Slider slider)
        {
            int index = SpeciesCreator.Instance.creator.genes.IndexOf(gene);
            prevValue = SpeciesCreator.Instance.creator.gene_weight[index];
            SpeciesCreator.Instance.creator.gene_weight[index] = slider.value;
        }
        public void Set(string gene)
        {
            this.gene = gene;
            label.text = $"{gene}";
            int index = SpeciesCreator.Instance.creator.genes.IndexOf(gene);
            prevValue = SpeciesCreator.Instance.creator.gene_weight[index];
            slider.value = prevValue;
        }
        public void RemoveGene()
        {
            SpeciesCreator.Instance.creator.RemoveGene(gene);
        }
    }
}