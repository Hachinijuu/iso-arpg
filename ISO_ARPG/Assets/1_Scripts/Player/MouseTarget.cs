using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// Targetting system will display tooltips on whatever is hovered by the mouse
// If the mouse is CLICKED on the hovered object, and it is a valid object, a target will be put on said object)
// Targets will be used for... Abilities, moving in range for melee attacks
// This system is mostly for SINGLE TARGET actions

public class MouseTarget : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] int raycastLength = 50;
    [SerializeField] List<string> tagsToMatch;
    //[SerializeField] DebugMenu dbMenu;
    public GameObject Target { get { return target; } }
    public LayerMask mask;

    // GameObjects
    public GameObject target;
    public GameObject mouseHit;


    // Player Variables
    PlayerInput input;
    #endregion
    #region UNITY FUNCTIONS

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        // MapActions();
    }

    // private void Start()
    // {
    //     // Call SetTarget here to get initial target text
    //     SetTarget();
    // }

    private void OnEnable()
    {
        MapActions();
    }

    private void OnDestroy()
    {
        UnmapActions();
    }
    void Update()
    {
        if (GameManager.Instance.currGameState == GameManager.GameState.PLAYING)
            UpdateMouseHover();
    }

    #endregion
    #region ACTION MAPPING
    private void MapActions()
    {
        input.actions["TargetLocked"].performed += context => { SetTarget(); };
    }

    private void UnmapActions()
    {
        input.actions["TargetLocked"].performed -= context => { SetTarget(); };
    }
    #endregion
    #region FUNCTIONALITY
    GameObject GetMouseHit()
    {
        // Raycast will be layer based, detecting only for
        // - Interactable objects
        // - Enemies
        mouseHit = null;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, raycastLength, mask))
        {
            //Collider hitCollider = hit.transform.gameObject.GetComponent<Collider>();

            // Build a bounding box
            foreach (string tag in tagsToMatch)
            {
                if (hit.transform.CompareTag(tag))                   // Check if the hit object was in a valid tag match
                {
                    mouseHit = hit.transform.gameObject;        // If it was, return what was hit.
                    target = mouseHit;  // Hover target
                }
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
            if (target.CompareTag("Interactable"))
            {
                InteractableObject interact = target.GetComponent<InteractableObject>();
                Debug.Log("Interact clicked");
                if (interact != null)
                {
                    if (interact.interactDistance > 0)
                        interact.OnInteract(gameObject.transform);
                    else
                        interact.OnInteract();
                }
            }

            //if (dbMenu)
            //    dbMenu.UpdateTargetText(target.name);
        }
        else
        {
            target = null;
            //if (dbMenu)
            //    dbMenu.UpdateTargetText(null);
        }
        Debug.Log(mouseHit);
    }

    void UpdateMouseHover()
    {
        GetMouseHit();
    }
    #endregion
}
