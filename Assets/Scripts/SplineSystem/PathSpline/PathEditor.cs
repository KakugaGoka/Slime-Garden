using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( PathSpline ) )]
public class PathEditor : Editor
{
    private void OnSceneGUI()
    {
        PathSpline spline = (PathSpline)target;

        ShowPoints( spline );
        EditorGUI.BeginChangeCheck();
        Event E = Event.current;
        switch (E.type) {
            case EventType.KeyDown:
                if (Event.current.keyCode == KeyCode.A) {
                    Undo.RecordObject( spline, "Add Point" );
                    spline.AddPoint( new Vector3( 0, 0, 0 ) );
                    EditorUtility.SetDirty( spline );
                }
                break;
        }
        EditorGUI.EndChangeCheck();
    }
    public override void OnInspectorGUI()
    {
        PathSpline spline = (PathSpline)target;
        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button( "Reset" )) {
            Undo.RecordObject( spline, "Reset" );
            spline.RemoveAllPoints();
            EditorUtility.SetDirty( spline );
        }
        EditorGUI.EndChangeCheck();
    }

    public static void ShowPoints( PathSpline target )
    {
        PathSpline spline = target;

        for (int i = 0; i < spline.points.Count; i++) {
            if (Selection.activeObject != spline.points[i]) {
                Vector3 point = spline.points[i].transform.position;
                float size = HandleUtility.GetHandleSize( point );

                Handles.color = new Color( 1, 1, 1, 0.2f );
                spline.points[i].transform.position =
                    Handles.FreeMoveHandle( point, Quaternion.identity, size * 0.5f, Vector3.zero, Handles.CircleHandleCap );
                Handles.color = Color.white;
                if (Handles.Button( point, Quaternion.identity, size * 0.1f, size * 0.1f, Handles.DotHandleCap )) {
                    Selection.activeObject = spline.points[i];
                }
            }
        }
    }
}