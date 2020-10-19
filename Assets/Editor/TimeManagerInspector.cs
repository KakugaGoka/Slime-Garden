using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimeManager))]
[CanEditMultipleObjects]
public class TimeManagerInspector : Editor
{
    int Day;
    int Hour;
    int Minute;

    float labelWidth = 150f;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUILayout.Space(20f);
        GUILayout.Label("Set World Time", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Day", GUILayout.Width(labelWidth));
        Day = EditorGUILayout.IntField(Day);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Hour", GUILayout.Width(labelWidth));
        Hour = EditorGUILayout.IntField(Hour);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Minute", GUILayout.Width(labelWidth));
        Minute = EditorGUILayout.IntField(Minute);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(labelWidth));
        if (GUILayout.Button("Set Time", GUILayout.Height(30f))) {
            TimeManager.main.SetTime(new Vector3(Day, Minute, Hour));
        }
        GUILayout.EndHorizontal();
    }
}
