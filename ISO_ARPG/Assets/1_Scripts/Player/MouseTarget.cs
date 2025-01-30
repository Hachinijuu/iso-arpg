using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// Targetting system will display tooltips on whatever is hovered by the mouse
// If the mouse is CLICKED on the hovered object, and it is a valid object, a target will be put on said object)
// Targets will be used for... Abilities, moving in range for melee attacks
// This system is mostly for SINGLE TARGET actions

public class MouseTarget : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<string> tagsToMatch;
    [SerializeField] DebugMenu dbMenu;
    public GameObject Target { get { return target; } }

    //[SerializeField] TMPro.TextMeshPro displayText;
    
    // GameObjects
    GameObject target;
    GameObject mouseHit;


    // Player Variables
    PlayerInput input;


    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        MapActions();
    }

    private void Start()
    {
        // Call SetTarget here to get initial target text
        SetTarget();
    }

    private void OnDestroy()
    {
        UnmapActions();
    }
    private void MapActions()
    {
        input.actions["TargetLocked"].performed += context => { SetTarget(); };
    }

    private void UnmapActions()
    { 
        input.actions["TargetLocked"].performed -= context => { SetTarget(); };
    }
    GameObject GetMouseHit()
    {
        // Raycast will be layer based, detecting only for
        // - Interactable objects
        // - Enemies
        mouseHit = null;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
        {
            //Debug.Log(hit.transform.tag);

            // Build a bounding box
            foreach (string tag in tagsToMatch)
            {
                if (hit.transform.tag == tag)                   // Check if the hit object was in a valid tag match
                    mouseHit = hit.transform.gameObject;        // If it was, return what was hit.
            }
        }
        return mouseHit;
    }

    void SetTarget()
    {
        // Target is set whenever a player CLICKS on a targettable object.
        // Need a way to remove the target selection (click on non?)
        if (mouseHit != null)
        {
            target = mouseHit;
            if (dbMenu)
                dbMenu.UpdateTargetText(target.name);
        }
        else
        { 
            target = null;
            if (dbMenu)
                dbMenu.UpdateTargetText(null);
        }

    }

    void UpdateMouseHover()
    {
        GetMouseHit();
        if (dbMenu != null)
        {
            if (mouseHit != null)
            {
                dbMenu.UpdateHoverText(mouseHit.name);
            }
            else
            {
                dbMenu.UpdateHoverText(null);
            }
        }
        //if (dbMenu != null)
        //{
        //if (GetMouseHit() != null)  // If the mouse hit returns a value, display it in the DebugMenu
        //{
        //    dbMenu.UpdateHoverText(mouseHit.name);
        //}
        //else
        //    dbMenu.UpdateHoverText(null);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.currGameState == GameManager.GameState.PLAYING)
            UpdateMouseHover();
    }
}
