using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SlimeController))]
[CanEditMultipleObjects]
public class SlimeInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        SlimeController slime = (SlimeController)target;

        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(150f));
        if (GUILayout.Button("Set Shader", GUILayout.Height(30f))) {
            slime.SetInShader();
        }
        GUILayout.EndHorizontal();
    }
}
