#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(PlanetObjectPlacementManager))]
class PlanetObjectPlacementManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        if (GUILayout.Button("Reallign Placed Objects")) {
            PlanetPlacedObject[] placedObjects = GameObject.FindObjectsByType<PlanetPlacedObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            Debug.Log("Alligning " + placedObjects.Length + " placed objects.");
            PlanetObjectPlacementManager manager = (PlanetObjectPlacementManager)target;
            
            for (int i = 0; i < placedObjects.Length; i++) {
                placedObjects[i].Reallign(manager.planetMask);
            }
        }
    }
}

#endif