using TMPro;

using UnityEngine;

namespace Simulation.Unity
{
    public class SpeciesStat : MonoBehaviour
    {
        public string title, txt, units;
        public TMP_Text label;
        public ProgressBar bar;
        int frames = 0;
        private void Update()
        {
            frames++;
            if(frames > 20)
            {
                frames = 0;
            }
            if(frames == 0 && !title.IsNullOrEmpty())
            {
                float value = Value();
                bar.currentValue = value / (GeneMod.max_ * 2f);
                label.text = $"{txt}: {value:F2} {units}";
            }
        }
        public float Value()
        {
            return SpeciesCreator.Instance.creator.GenePotential.Get(title);
        }
        public void Set(string gene)
        {
            title = gene;
            var f = Field();
            txt = f.newName;
            units = f.units;
            bar.simpleBar.minColor = f.color;
            bar.simpleBar.maxColor = f.color;
        }
        public GeneMenu.FieldReMod Field()
        {
            return GeneMenu.Instance.UI.fieldMods.Find( f => f.name == title);
        }
    }
}