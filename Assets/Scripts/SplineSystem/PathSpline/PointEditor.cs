using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor( typeof( PathPoint ) )]
public class PointEditor : Editor
{
    private void OnSceneGUI()
    {
        PathPoint point = (PathPoint)target;

        int pointIndex = point.transform.GetSiblingIndex();
        PathSpline parentSpline = point.transform.parent.GetComponent<PathSpline>();

        PathEditor.ShowPoints( parentSpline );

        EditorGUI.BeginChangeCheck();
        Event E = Event.current;
        switch (E.type) {
            case EventType.KeyDown:
                if (Event.current.keyCode == KeyCode.A) {
                    Undo.RecordObject( parentSpline, "Add Point" );
                    parentSpline.AddPoint( new Vector3( 0, 0, 0 ) );
                    EditorUtility.SetDirty( parentSpline );
                }
                break;
        }
        EditorGUI.EndChangeCheck();
    }
}