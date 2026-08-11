using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteAlways]
public class ToggleV2 : MonoBehaviour
{
    public Transform target;
    public Image graphic;
    public TMP_Text text;
    public UnityEvent onValueChanged;
    public bool isOn;
    public bool IsOn
    {
        get
        {
            return isOn;
        }
        set
        {
            if(value != isOn)
            {
                ToggleOn();
            }
        }
    }
    public Vector2[] states;
    public Color[] colors;
    public string[] texts;
    private void Awake()
    {
        OnToggleChanged();
    }
    int prev = -1;
    private void Update()
    {
        if(isOn && prev != 1)
        {
            OnToggleChanged();
            prev = 1;
        }else if(!isOn && prev != 0)
        {
            OnToggleChanged();
            prev = 0;
        }
    }
    public void ToggleOn()
    {
        isOn = !isOn;
        if(Application.isPlaying)
        {
            onValueChanged.Invoke();
        }
        OnToggleChanged();
    }
    public void OnToggleChanged()
    {
        target.localPosition = states[isOn ? 1 : 0];
        graphic.color = colors[isOn ? 1 : 0];
        if(text)
        {
            text.text = texts[isOn ? 1 : 0];
        }
    }
}

