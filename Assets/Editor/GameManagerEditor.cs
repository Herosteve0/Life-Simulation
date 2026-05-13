using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GameManager script = (GameManager)target;

        if (GUILayout.Button("Reset Simulation")) {
            script.ResetSimulation();
        }
    }
}
