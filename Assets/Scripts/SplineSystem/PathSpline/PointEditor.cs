﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.PlayerLoop;

[CustomEditor( typeof( PathPoint ) )]
public class PointEditor : Editor
{
    private void OnSceneGUI()
    {
        PathPoint point = (PathPoint)target;

        int pointIndex = point.transform.GetSiblingIndex();

        PathSpline parentSpline = point.transform.parent.GetComponent<PathSpline>();

        parentSpline.selectedIndex = pointIndex;

        PathEditor.ShowPoints( parentSpline );

        EditorGUI.BeginChangeCheck();
        Event E = Event.current;
        switch (E.type) {
            case EventType.KeyDown:
                if (Event.current.keyCode == KeyCode.Tab) {
                    Undo.RecordObject( parentSpline, "Add Point" );
                    parentSpline.AddPoint();
                    EditorUtility.SetDirty( parentSpline );
                }
                break;
        }
        EditorGUI.EndChangeCheck();
    }
}