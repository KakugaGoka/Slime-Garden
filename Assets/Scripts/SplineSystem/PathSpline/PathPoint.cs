using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[System.Serializable]
public class PathPoint : MonoBehaviour
{
    public Vector3 handleA {
        get { return transform.TransformPoint( localA ); }
        set { localA = transform.InverseTransformPoint( value ); }
    }
    public Vector3 handleB {
        get { return transform.TransformPoint( localB ); }
        set { localB = transform.InverseTransformPoint( value ); }
    }
    public Vector3 localA;
    public Vector3 localB;
    public BezierControlPointMode mode = BezierControlPointMode.Mirrored;
    private void Awake()
    {
        localA = Vector3.back;
        localB = Vector3.forward;
    }
    private void Start()
    {
        var spline = transform.parent.GetComponent<PathSpline>();
        spline.RenamePoints();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere( transform.position, 0.1f );
    }

    private void OnDestroy()
    {
        var spline = transform.parent.GetComponent<PathSpline>();
        Undo.RecordObject( spline, "Delete Point" );
        spline.points.Remove( gameObject );

        spline.RenamePoints();
    }
}