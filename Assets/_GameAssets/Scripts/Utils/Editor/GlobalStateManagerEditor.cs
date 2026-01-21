#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GlobalStateManager))]
public class GlobalStateManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GlobalStateManager gsm = (GlobalStateManager)target;
        if (GUILayout.Button("Reset Progress"))
        {
            gsm.ResetProgress();
            Debug.Log("GlobalStateManager progress reset.");
        }
    }
}

#endif