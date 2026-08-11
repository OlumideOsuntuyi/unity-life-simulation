using TMPro;

using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ScreenManager : Singleton<ScreenManager>
{
    public string activeScreenID = null;
    private string prevActiveID = null;
    public ScreenControl mainScreenControl;
    public ScreenControl mainMenuScreenControl;
    public ScreenControl activeBox;
    public ScreenBools screenBool;
    private void Update()
    {
        if(prevActiveID != activeScreenID)
        {
            prevActiveID = activeScreenID;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
    }
    public void SetWorldCam(bool activate)
    {
        screenBool.canMoveWorldCamera = activate;
    }
    public void SwitchMainScreen(bool activate)
    {
        SetWorldCam(activate);
    }
    public void SwitchIfBarIsZero(ScreenControl box)
    {
        SwitchMainScreen(box.active_box == 0 && mainScreenControl.active_box == 0);
    }
    public void Return()
    {
        if(activeBox != null)
        {
            if(activeBox == activeBox.parent)
            {
                activeBox.SetActiveBox(0);
            }
            else
            {
                if(activeBox.doParentAfterZero && activeBox.active_box != 0)
                {
                    activeBox.SetActiveBox(0);
                }
                else
                {
                    activeBox.SetActiveBox(0);
                    if (activeBox.changeParentIndex)
                    {
                        if (activeBox.parent == null)
                        {
                            activeBox.parent = mainScreenControl;
                        }
                        activeBox.parent.active_box = activeBox.parentIndex;
                    }
                    activeBox = activeBox.parent;
                }
            }
            //SwitchMainScreen(mainScreenControl.active_box is 0 && overlayControl.active_box is 0);
        }
    }
    [System.Serializable]
    public class ErrorUI
    {
        public TMP_Text errorMessage;
        public GameObject gameObject;
        public int index;
    }
    [System.Serializable]
    public struct ScreenBools
    {
        public bool canMoveWorldCamera;
    }
}
public enum ScreenState { }
