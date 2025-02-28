using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUtility : MonoBehaviour
{
    // Move camera shift functions to this class instead of requiring a reference to the HUD controller

    #region Toggle Setup
    // Multipurpose to be implemented in editor
    // When clicked it sets the specified component to active displaying the onscreen element
    public static void ToggleUIElement(GameObject toToggle)
    {
        if (!toToggle.activeInHierarchy)    // If it is not active, turn it on
            toToggle.SetActive(true);
        else                                // If it is active, shut it off
            toToggle.SetActive(false);
    }

    #region Camera Toggling
    public static void ToggleUIElementShift(GameObject toToggle, Camera cam = null)
    {
        RectTransform rt = toToggle.GetComponent<RectTransform>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (rt != null)
        {
            Rect newCam = new Rect(0, 0, 1, 1);
            if (!toToggle.activeInHierarchy)    // If it is not active, turn it on
            {
                toToggle.SetActive(true);
                newCam.x -= (rt.anchorMax.x - rt.anchorMin.x);
            }
            else                                // If it is active, shut it off
            {
                toToggle.SetActive(false);
            }

            cam.rect = newCam;
        }
    }

    public static void ShiftCamera(GameObject offset, Camera cam = null)
    {
        RectTransform rt = offset.GetComponent<RectTransform>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (rt != null)
        {
            // do some math to setup camera
            Rect newCam = new Rect(0, 0, 1, 1);
            newCam.x -= (rt.anchorMax.x - rt.anchorMin.x);
            cam.rect = newCam;
        }
    }
    #endregion
    #endregion
}
