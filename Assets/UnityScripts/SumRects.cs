using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SumRects : MonoBehaviour
{
    public RectTransform target, content;
    public Vector2 init, spacing;
    public bool x, y;
    public bool doVerticalLayout;
    public Padding padding;
    int frame = 0;
    private void LateUpdate()
    {
        frame++;
        if (frame < 10)
        {
            frame = 0;
        }
        if (target && content && frame == 0)
        {
            target.sizeDelta = init;
            Vector3 next = Vector3.zero;
            if(content.childCount > 0)
            {
                for (int i = 0; i < content.childCount; i++)
                {
                    var rect = content.GetChild(i).GetComponent<RectTransform>();
                    if (rect.gameObject.activeInHierarchy)
                    {
                        target.sizeDelta += new Vector2(x ? rect.sizeDelta.x : 0, y ? rect.sizeDelta.y : 0) + spacing;

                        if (doVerticalLayout)
                        {
                            rect.localPosition = next + new Vector3(padding.left - padding.right, padding.top - padding.bottom, 0);
                            next -= new Vector3(0, doVerticalLayout ? rect.sizeDelta.y + spacing.y : 0, 0);
                        }
                    }
                }
            }
        }
    }

    [System.Serializable]
    public struct Padding
    {
        public float top, left, right, bottom;
    }
}
