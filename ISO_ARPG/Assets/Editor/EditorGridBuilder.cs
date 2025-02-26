using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(GridBuilder))]
public class EditorGridBuilder : Editor
{
    bool canMarkCells;
    GridBuilder builder;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        builder = (GridBuilder)target;
        
        if (GUILayout.Button("Build Grid"))
        {
            builder.BuildGrid();
        }
        if (GUILayout.Button("Clear Obstacles"))
        {
            builder.CullObstacles();
        }
        if (GUILayout.Button("Save Grid"))
        {
            builder.SaveGrid();
        }
        GUILayout.Label("Customization");
        canMarkCells = GUILayout.Toggle(canMarkCells, "Mark Unusable Cells");
        if (canMarkCells)
        {
            builder.cellMarker = true;
        }
        else
        {
            builder.cellMarker = false;
        }
        if (GUILayout.Button("Clear Removed Tiles"))
        {
            builder.ClearRemovedList();
        }
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += context => { OnSceneGUI(); };
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= context => { OnSceneGUI(); };
    }

    Vector3 startPos;
    Vector3 endPos;
    Vector3 centre;
    Vector3 size;

    bool dragging;
    bool removeMode;

    private void OnSceneGUI() 
    {
        if (canMarkCells)
        {
            HandleDrag();
            if (dragging)
            {
                GetBox(ref centre, ref size);
                Handles.DrawWireCube(centre, size);
                HandleRemoval(removeMode);
            }
        }
    }

    List<Cell> ghostMarker = new List<Cell>();
    private void HandleRemoval(bool remove)
    {
        // Single vs Drag

        //if (startPos == endPos) // Single clicked
        //{
        //    GridUtility.GetCellFromIndex();
        //}
        //Debug.Log(dragging);
        Bounds box = new Bounds(centre, size);
        builder.MarkClickedCell(box, dragging, ref ghostMarker, remove);
        builder.UpdateGhostCells(ghostMarker);
    }
    private void HandleDrag()
    {
        Event e = Event.current;
        if (e.shift)
            removeMode = false; // DON'T WANT TO REMOVE, WANT TO REPAIR
        else
            removeMode = true;  // WANT TO REMOVE
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit))
            {
                dragging = false;
                startPos = hit.point;
                endPos = startPos;
            }
            e.Use();
        }
        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            dragging = true;
            Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit))
            {
                endPos = hit.point;
            }
            e.Use();
        }
        if (e.type == EventType.MouseUp && e.button == 0)
        {
            dragging = false;
            HandleRemoval(removeMode);
            // Debug.Log("Locked the grid");
            e.Use();
        }    
    }

    private void GetBox(ref Vector3 centre, ref Vector3 size)
    {
        float length = startPos.x - endPos.x;
        float width = startPos.z - endPos.z;

        centre = startPos - new Vector3(length / 2, 0, width / 2);
        size = new Vector3(length, 0.1f, width);
    }
}
