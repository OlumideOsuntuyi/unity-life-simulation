using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Simulation.Unity
{
    public class GeneMod : MonoBehaviour
    {
        public const float min_ = 0f;
        public const float max_ = 10f;
        public TMP_Text label;
        public TMP_Text min;
        public TMP_Text max;
        public TMP_Text current;
        public Slider selector;
        public string title { get; private set; }
        public float value => Math.Lerp(min_, max_, selector.value);
        float prevValue = -1;
        private void Update()
        {
            if(GeneMenu.Instance.EnergyLeftForEdit() < (value - Mathf.Lerp(min_, max_, prevValue)))
            {
                selector.value = prevValue;
            }
        }
        private void LateUpdate()
        {
            if (!title.IsNullOrEmpty() && selector.value != prevValue)
            {
                current.text = value.ToString("F2");
                prevValue = selector.value;
                SaveGene();
            }
        }
        public void Set(string name, float value, GeneMenu.FieldReMod label)
        {
            this.title = name;
            this.label.text = label.newName;
            selector.value = Mathf.InverseLerp(min_, max_, value);
            this.label.color = label.color;
            selector.value = 0;
            min.text = StaticShortcuts.ReduceNumberStringLength(0);
            max.text = StaticShortcuts.ReduceNumberStringLength(10);
        }
        public void SaveGene()
        {
            GeneMenu.Instance.EditMod(this);
        }
        public void Remove()
        {
            GeneMenu.Instance.RemoveMod(this);
        }
    }
}