using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Simulation.Unity
{
    public class SpeciesInfo : MonoBehaviour
    {
        public Color[] colors;
        public Image background;
        public TMP_Text speciesName;
        public TMP_Text population;
        public TMP_InputField nameInput;
        public RectTransform content;
        public List<int> sourceGenes;
        public SourceGeneButton sourceGenePrefab;
        private void Awake()
        {
            sourceGenes = new();
        }
        public void ListSourceGenes()
        {

        }
        public void AddSourceGene(int index)
        {
            sourceGenes.Add(index);
            ListSourceGenes();
        }
        public void Remove(int index)
        {
            sourceGenes.Remove(index);
            ListSourceGenes();
        }
    }
}