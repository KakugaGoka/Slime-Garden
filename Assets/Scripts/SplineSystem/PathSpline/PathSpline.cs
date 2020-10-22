using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[System.Serializable]
public class PathSpline : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> points = new List<GameObject>();

    public BezierSpline spline;
    private int resolution = 5;

    private void Awake()
    {
        AddPoint( transform.position );
    }
    private void OnEnable()
    {
    }

    private void OnDrawGizmos()
    {
        //if (spline.points != null) {
        //    if (spline.points.Count > 3) {
        //        Gizmos.color = Color.white;

        //        float step = 1 / (float)(points.Count * resolution);
        //        for (float t = 0; t < 1; t += step) {
        //            Gizmos.DrawLine( spline.GetPoint( t ), spline.GetPoint( t + step ) );
        //        }
        //    }
        //}
    }
    public void AddPoint( Vector3 point )
    {
        var pointObj = new GameObject();
        pointObj.AddComponent<PathPoint>();
        pointObj.name = (points.Count + 1).ToString();
        pointObj.transform.parent = transform;
        pointObj.transform.position = point;
        points.Add( pointObj );

        Undo.RegisterCreatedObjectUndo( pointObj, "Create Point" );

        RenamePoints();
    }
    public void UpdateCurve()
    {
        if (spline) {
        }
        spline.points.Clear();
        spline.modes.Clear();
        for (int i = 0; i < points.Count; i++) {
            spline.AddCurve( points[i].transform.position );
        }
    }

    public void RenamePoints()
    {
        for (int i = 0; i < points.Count; i++) {
            points[i].name = "Point " + i;
        }
    }
    public void RemovePoint( int index )
    {
        //var point = points[index];

        Undo.DestroyObjectImmediate( points[index] );
        points.TrimExcess();

        RenamePoints();
    }

    public void RemoveAllPoints()
    {
        for (int i = points.Count - 1; i >= 0; i--) {
            RemovePoint( i );
        }
    }

    private void Update()
    {
        if (spline == null) {
            //AddPoint( Vector3.zero );
            //AddPoint( Vector3.one );

            spline = new BezierSpline();

            //spline.AddCurve( Vector3.zero );
            //spline.AddCurve( Vector3.one );
        }
        if (points.Count > 2) {
            UpdateCurve();
        }
        else {
        }
    }
}