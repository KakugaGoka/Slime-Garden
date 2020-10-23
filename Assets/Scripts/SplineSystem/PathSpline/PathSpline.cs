using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using static Mathk;
using System.Runtime.InteropServices.ComTypes;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

[System.Serializable]
public class PathSpline : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> points = new List<GameObject>();

    public PathPoint this[int i] => points[i].GetComponent<PathPoint>();

    public BezierSpline spline;

    [SerializeField]
    [Range( (int)1, (int)50 )]
    public int resolution;
    public int selectedIndex = 0;
    public bool looping;
    public bool showGizmo;
    private void Awake()
    {
        resolution = 5;
        AddPoint();
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
        if (showGizmo) {
            Vector3 start;
            Vector3 end;
            for (int i = 0; i < points.Count - 1 + Convert.ToInt32( looping ); i++) {
                PathPoint point = points[i].GetComponent<PathPoint>();
                PathPoint next = points[(i + 1) % points.Count].GetComponent<PathPoint>();
                float res = 1 / (float)(resolution);
                for (float t = 0; t < 1; t += res) {
                    start = Bezier.GetPoint( point.transform.position, point.handleB, next.handleA, next.transform.position, t );
                    end = Bezier.GetPoint( point.transform.position, point.handleB, next.handleA, next.transform.position, t + res );
                    Gizmos.DrawLine( start, end );
                }
            }
        }
    }

    public void AddPoint()
    {
        var pointObj = new GameObject();
        var pointCom = pointObj.AddComponent<PathPoint>();
        pointObj.name = (points.Count + 1).ToString();
        pointObj.transform.parent = transform;

        Vector3 point;

        switch (selectedIndex) {
            case -1:
                point = gameObject.transform.position;
                points.Add( pointObj );
                break;

            case int i when i == points.Count:
                point = points[selectedIndex].transform.position;
                pointCom.localB = GetDx( selectedIndex + 0.5f );
                pointCom.localA = -pointCom.localB;
                points.Add( pointObj );
                break;

            default:
                point = GetPoint( selectedIndex + 0.5f );
                pointCom.localB = GetDx( selectedIndex + 0.5f );
                pointCom.localA = -pointCom.localB;
                points.Insert( selectedIndex + 1, pointObj );
                break;
        }

        pointObj.transform.position = point;

        Undo.RegisterCreatedObjectUndo( pointObj, "Create Point" );

        RenamePoints();
    }
    public void RenamePoints()
    {
        for (int i = 0; i < points.Count; i++) {
            points[i].transform.SetSiblingIndex( i );
            points[i].name = i.ToString();
        }
    }
    public void RemovePoint( int index )
    {
        //var point = points[index];

        Destroy( points[index] );
        points.TrimExcess();

        RenamePoints();
    }

    public void RemoveAllPoints()
    {
        for (int i = points.Count - 1; i >= 0; i--) {
            RemovePoint( i );
        }
    }

    public Vector3 GetPoint( float t )
    {
        int lobound = Mathf.FloorToInt( Clamp( t, 0, points.Count - 1 ) );
        int hibound = Clamp( lobound + 1, 0, points.Count - 1 );

        PathPoint lopoint = points[lobound].GetComponent<PathPoint>();
        PathPoint hipoint = points[hibound].GetComponent<PathPoint>();

        float remainder = t - lobound;

        return Bezier.GetPoint( lopoint.transform.position,
                               lopoint.handleB,
                               hipoint.handleA,
                               hipoint.transform.position,
                               remainder );
    }

    public Vector3 GetDx( float t )
    {
        int lobound = Mathf.FloorToInt( Clamp( t, 0, points.Count - 1 ) );
        int hibound = Clamp( lobound + 1, 0, points.Count - 1 );

        PathPoint lopoint = points[lobound].GetComponent<PathPoint>();
        PathPoint hipoint = points[hibound].GetComponent<PathPoint>();

        float remainder = t - lobound;

        return Bezier.GetFirstDerivative( lopoint.transform.position,
                               lopoint.handleB,
                               hipoint.handleA,
                               hipoint.transform.position,
                               remainder );
    }

    //private List<float> distances = new List<float>();
    //private List<Vector3> localMinima = new List<Vector3>();
    private Vector3 loBoundPoint;
    private Vector3 loBoundNormal;
    private Vector3 hiBoundPoint;
    private Vector3 hiBoundNormal;
    private float t;
    public int subd = 3;
    public Vector3 GetClosestPoint( Vector3 WSPoint )
    {
        List<float> distances = new List<float>();
        List<Vector3> localMinima = new List<Vector3>();
        float res = 1 / (float)resolution;
        for (int i = 0; i < points.Count - 1; i++) {
            for (float n = i; n < i + 1; n += res) {
                GetSplineBounds( res, n );
                if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                    for (float d = n; d < n + res; d += res / (float)subd) {
                        GetSplineBounds( res / (float)subd, d );

                        if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                            for (float p = d; p < d + (res / (float)subd); p += res / (float)(subd * subd)) {
                                GetSplineBounds( res / (float)(subd * subd), p );

                                if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                                    for (float l = p; l < p + (res / (float)(subd * subd)); l += res / (float)(subd * subd * subd)) {
                                        GetSplineBounds( res / (float)(subd * subd * subd), l );

                                        if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                                            t = MeasureBetweenPlaneBounds( WSPoint,
                                                                           loBoundPoint,
                                                                           loBoundNormal,
                                                                           hiBoundPoint,
                                                                           hiBoundNormal );

                                            localMinima.Add( Vector3.LerpUnclamped( loBoundPoint, hiBoundPoint, t ) );
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        localMinima.Add( this[0].transform.position );
        localMinima.Add( this[points.Count - 1].transform.position );

        for (int i = 0; i < localMinima.Count; i++) {
            distances.Add( Vector3.Distance( localMinima[i], WSPoint ) );
        }

        for (int i = 0; i < localMinima.Count; i++) {
            if (i == distances.FindIndex( ( x ) => x == Min( distances ) )) {
                return localMinima[i];
            }
        }
        Debug.LogWarning( "Closest spline point not found." );
        return Vector3.zero;

        void GetSplineBounds( float step, float t )
        {
            loBoundPoint = GetPoint( t );
            loBoundNormal = GetDx( t ).normalized;
            hiBoundPoint = GetPoint( t + step );
            hiBoundNormal = GetDx( t + step ).normalized;
        }
    }
}