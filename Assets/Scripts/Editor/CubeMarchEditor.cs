using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CubeMarch))]
public class CubeMarchEditor : Editor {

    public override void OnInspectorGUI() {
        CubeMarch cm = (CubeMarch)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate")) {
            cm.Generate();
        }
    }
}
