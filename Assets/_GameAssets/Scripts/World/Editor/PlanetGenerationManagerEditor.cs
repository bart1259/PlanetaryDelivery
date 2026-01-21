#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PlanetGenerationManager))]
public class PlanetGenerationManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlanetGenerationManager planetGenManager = (PlanetGenerationManager)target;
        if (GUILayout.Button("Generate Planet Mesh"))
        {
            planetGenManager.GeneratePlanetMesh();
        }
    }
}


#endif