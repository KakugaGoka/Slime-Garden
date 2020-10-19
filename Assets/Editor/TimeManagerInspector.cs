using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimeManager))]
[CanEditMultipleObjects]
public class TimeManagerInspector : Editor
{
    SerializedProperty currentHourProperty;
    SerializedProperty currentDayProperty;

    public void OnEnable() {
        currentHourProperty = serializedObject.FindProperty("currentHour");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    
        if(GUILayout.Button("SetTime")) {
            TimeManager main = TimeManager.main;
            main.SetTime(main.dayHourMinute);
        }
    }
}
