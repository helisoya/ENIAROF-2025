using UnityEngine;

public class MultiDisplaySetup : MonoBehaviour
{
    void Start()
    {
        // active ecran dispo
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        //def resolution et main screen
        if (Display.displays.Length > 1)
        {
            Screen.SetResolution(1920, 1080, true, 60);
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
    }
}