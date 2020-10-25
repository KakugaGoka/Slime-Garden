using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using static Mathk;
using System;
using UnityEditor.ShaderGraph.Internal;

[CustomEditor( typeof( PathSpline ) )]
public class PathEditor : Editor
{
    private static bool alt = false;

    private void OnSceneGUI()
    {
        PathSpline spline = (PathSpline)target;

        ShowPoints( spline );

        EditorGUI.BeginChangeCheck();
        Event E = Event.current;
        switch (E.type) {
            case EventType.KeyDown:
                if (Event.current.keyCode == KeyCode.Tab) {
                    Undo.RecordObject( spline.gameObject, "Add Point" );
                    spline.AddPoint();
                    EditorUtility.SetDirty( spline.gameObject );
                }
                break;
        }
        EditorGUI.EndChangeCheck();
    }

    public static void ShowPoints( PathSpline target )
    {
        PathSpline spline = target;
        float[] distances = new float[spline.points.Count];

        if (spline.points.Contains( Selection.activeGameObject )) {
            spline.selectedIndex = spline.points.FindIndex( ( x ) => x == Selection.activeGameObject );
        }
        else {
            spline.selectedIndex = spline.points.Count - 1;
        }

        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftAlt) {
            alt = true;
        }
        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftAlt) {
            alt = false;
        }

        if (!alt) {
            for (int i = 0; i < spline.points.Count; i++) {
                if (spline.points[i] != null) {
                    Handles.color = new Color( 1, 1, 1, 0.2f );

                    Vector3 oldpos = spline.points[i].transform.position;
                    Vector3 loc = spline.points[i].transform.position;
                    Quaternion rot = spline.points[i].transform.rotation;
                    Vector3 scale = spline.points[i].transform.localScale;
                    float size = HandleUtility.GetHandleSize( loc );

                    if (spline.points[i] != Selection.activeObject) {
                        Undo.RecordObject( spline.points[i].gameObject.transform, "Edit Point" );

                        spline.points[i].transform.position =
                        Handles.FreeMoveHandle( i, loc, rot, size * 0.5f, Vector3.zero, Handles.CircleHandleCap );

                        EditorUtility.SetDirty( spline.points[i].gameObject.transform );

                        Undo.RecordObject( spline.points[i].GetComponent<PathPoint>(), "Edit Handles" );
                        DrawHandles( spline, i, new Color( 1, 1, 1, 0.2f ) );
                        EditorUtility.SetDirty( spline.points[i].GetComponent<PathPoint>() );

                        if (Handles.Button( loc,
                            Quaternion.LookRotation( loc - Camera.current.transform.position ),
                            size * 0.1f,
                            size * 0.1f,
                            Handles.DotHandleCap )) {
                            Selection.activeGameObject = spline.points[i];
                        }
                    }
                    else {
                        Undo.RecordObject( spline.points[i].gameObject.transform, "Edit Point" );
                        spline.points[i].transform.position =
                                Handles.FreeMoveHandle( i, loc, rot, size * 0.5f, Vector3.zero, Handles.CircleHandleCap );
                        EditorUtility.SetDirty( spline.points[i].gameObject.transform );

                        Undo.RecordObject( spline.points[i].GetComponent<PathPoint>(), "Edit Handles" );
                        DrawHandles( spline, i, Color.white );
                        EditorUtility.SetDirty( spline.points[i].GetComponent<PathPoint>() );
                    }
                }
            }
        }
        void DrawHandleA( PathPoint point )
        {
            point.handleA =
                Handles.FreeMoveHandle(
                    point.transform.TransformPoint( point.localA ),
                    Quaternion.identity,
                    HandleUtility.GetHandleSize( point.handleA ) * 0.07f,
                    Vector3.zero,
                    Handles.DotHandleCap
                    );
            point.localB = -point.localA;
        }
        void DrawHandleB( PathPoint point )
        {
            point.handleB =
                Handles.FreeMoveHandle(
                    point.transform.TransformPoint( point.localB ),
                    Quaternion.identity,
                    HandleUtility.GetHandleSize( point.handleB ) * 0.07f,
                    Vector3.zero,
                    Handles.DotHandleCap
                    );
            point.localA = -point.localB;
        }

        void DrawHandles( PathSpline inSpline, int i, Color color )
        {
            Handles.color = color;

            int last = inSpline.points.Count - 1;

            PathPoint point = inSpline.points[i].GetComponent<PathPoint>();

            if (inSpline.looping) {
                DrawHandleA( point );
                Handles.DrawLine( point.transform.position, point.handleA );
                DrawHandleB( point );
                Handles.DrawLine( point.transform.position, point.handleB );
            }
            else {
                switch (i) {
                    case 0:
                        DrawHandleB( point );
                        Handles.DrawLine( point.transform.position, point.handleB );
                        break;

                    case int p when p == last:
                        DrawHandleA( point );
                        Handles.DrawLine( point.transform.position, point.handleA );
                        break;

                    default:
                        DrawHandleA( point );
                        Handles.DrawLine( point.transform.position, point.handleA );
                        DrawHandleB( point );
                        Handles.DrawLine( point.transform.position, point.handleB );
                        break;
                }
            }
        }
    }
}