using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[System.Serializable]
public class PathSpline : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> points = new List<GameObject>();

    public BezierSpline spline;

    [SerializeField]
    [Range( (int)1, (int)50 )]
    public int resolution;
    public int selectedIndex = 0;
    public bool looping;
    private void Awake()
    {
        resolution = 5;
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
        Vector3 start;
        Vector3 end;
        for (int i = 0; i < points.Count - 1 + Convert.ToInt32( looping ); i++) {
            PathPoint point = points[i].GetComponent<PathPoint>();
            PathPoint next = points[(i + 1) % points.Count].GetComponent<PathPoint>();
            float step = 1 / (float)(resolution);
            for (float t = 0; t < 1; t += step) {
                start = Bezier.GetPoint( point.transform.position, point.handleB, next.handleA, next.transform.position, t );
                end = Bezier.GetPoint( point.transform.position, point.handleB, next.handleA, next.transform.position, t + step );
                Gizmos.DrawLine( start, end );
            }
        }
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
    }
}