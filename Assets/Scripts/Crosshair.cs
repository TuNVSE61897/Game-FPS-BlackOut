using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the placement and drawing of the retical in the center of the screen
public class Crosshair : MonoBehaviour
{
    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), "");
    }
}
