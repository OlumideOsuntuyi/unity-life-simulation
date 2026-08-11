
using TMPro;

using UnityEngine;

namespace Simulation.Unity
{
    public class SourceGeneButton: MonoBehaviour
    {
        public SpeciesInfo src;
        public TMP_Text title;
        public int geneIndex;
        public void Set(int index)
        {
            geneIndex = index;
        }
        public void Click()
        {
            src.AddSourceGene(geneIndex);
        }
    }
}