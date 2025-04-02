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

    private static Rect originalRect = new Rect(0,0,1,1);
    private static Dictionary<GameObject, float> UIElements = new Dictionary<GameObject, float>();

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
            if (!toToggle.activeInHierarchy)    // If it is not active, turn it on
            {
                if (UIElements.Count == 0)      // If there are NO UI ELEMENTS
                {
                    originalRect = cam.rect;    // Store th eoriginal rect
                }

                toToggle.SetActive(true);       // Activate the element

                float camOffset = 0f;
                if (rt.anchorMin.x == 0)
                {
                    camOffset = rt.anchorMax.x;
                }
                else
                {
                    camOffset = -rt.anchorMin.x;
                    //newCam.x -= (1- rt.anchorMin.x);
                }

                // If the dictionary DOES NOT contain the element that was just toggled on
                if (!UIElements.ContainsKey(toToggle))
                {
                    UIElements.Add(toToggle, camOffset);    // Add the element with the offset value, this is used for proper shift resets
                }

                UpdateCamera(cam);
                //newCam.x -= (rt.anchorMax.x - rt.anchorMin.x);
                //cam.rect = newCam;
            }
            else                                // If it is active, shut it off
            {
                toToggle.SetActive(false);
                if (UIElements.ContainsKey(toToggle))
                {
                    UIElements.Remove(toToggle);
                }
                UpdateCamera(cam);
            }
        }
    }

    public static void UpdateCamera(Camera cam)
    {
        float offset = 0f;
        foreach (float camShift in UIElements.Values)
        {
            offset += camShift;
        }

        Rect newCam = originalRect;
        newCam.x += offset;
        cam.rect = newCam;
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
