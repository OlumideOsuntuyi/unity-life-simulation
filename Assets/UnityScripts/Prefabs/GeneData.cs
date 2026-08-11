using TMPro;

using UnityEngine;

namespace Simulation.Unity
{
    public class GeneData : MonoBehaviour
    {
        public TMP_Text label, energyCost;

        public void Set(GenePotential gene)
        {
            label.text = gene.name;
            energyCost.text = $"Energy Cost: {gene.Weight():F2}";
        }
    }
}