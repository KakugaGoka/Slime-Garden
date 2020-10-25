using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using static Mathk;
using System.Runtime.InteropServices.ComTypes;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine.Events;

[System.Serializable]
[ExecuteInEditMode]
public class PathSpline : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> points = new List<GameObject>();

    public PathPoint this[int i] => points[i].GetComponent<PathPoint>();

    public BezierSpline spline;

    [SerializeField]
    [Range( (int)1, (int)50 )]
    public int resolution = 5;
    public int subdivision = 5;
    public int selectedIndex = 0;
    public bool looping;
    public bool showGizmo = true;
    public bool showRaw = true;
    private void Awake()
    {
    }
    private void OnEnable()
    {
    }
    private void Update()
    {
        if (transform.childCount != points.Count) {
            points.Clear();
            for (int i = 0; i < transform.childCount; i++) {
                if (transform.GetChild( i ).gameObject != null) {
                    points.Add( transform.GetChild( i ).gameObject );
                }
            }
        }
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
                if (points[i] != null && points[i + 1] != null) {
                    PathPoint point = points[i].GetComponent<PathPoint>();
                    PathPoint next = points[(i + 1) % points.Count].GetComponent<PathPoint>();
                    float res = 1 / (float)(resolution);
                    for (float t = 0; t < 1; t += res) {
                        start = Bezier.GetPoint( point.transform.position, point.handleB, next.handleA, next.transform.position, t );
                        end = Bezier.GetPoint( point.transform.position, point.handleB, next.handleA, next.transform.position, t + res );
                        Gizmos.DrawLine( start, end );
                    }
                }
                else {
                    RemoveNullPoints();
                }
            }
        }
    }
    public void RemoveNullPoints()
    {
        for (int i = points.Count - 1; i >= 0; i--) {
            if (points[i] == null) {
                points.RemoveAt( i );
            }
        }
        RenamePoints();
    }
    public void RenamePoints()
    {
        if (points != null) {
            for (int i = 0; i < points.Count; i++) {
                points[i].transform.SetSiblingIndex( i );
                points[i].name = i.ToString();
            }
        }
    }
    public void AddPoint()
    {
        RemoveNullPoints();
        var pointObj = new GameObject();
        var pointCom = pointObj.AddComponent<PathPoint>();
        pointObj.name = (points.Count + 1).ToString();
        pointObj.transform.parent = transform;

        Vector3 point;

        switch (selectedIndex) {
            case -1:
                point = gameObject.transform.position;
                points.Add( pointObj );
                pointCom.localB = Vector3.forward;
                pointCom.localA = -pointCom.localB;
                break;

            case int i when i == points.Count - 1:
                point = points[selectedIndex].transform.position;
                pointCom.localB = GetDx( selectedIndex + 0.5f ) * 0.1f;
                pointCom.localA = -pointCom.localB;
                points.Add( pointObj );
                break;

            default:
                point = GetPoint( selectedIndex + 0.5f );
                pointCom.localB = GetDx( selectedIndex + 0.5f ) * 0.1f;
                pointCom.localA = -pointCom.localB;
                points.Insert( selectedIndex + 1, pointObj );
                break;
        }

        pointObj.transform.position = point;

        Undo.RegisterCreatedObjectUndo( pointObj, "Create Point" );

        RenamePoints();
    }

    public void RemovePoint( int index )
    {
        //points.RemoveAt( index );

        RemoveNullPoints();

        //Destroy( points[index] );
        points.TrimExcess();
    }

    public void RemoveAllPoints()
    {
        for (int i = points.Count - 1; i >= 0; i--) {
            RemovePoint( i );
            Destroy( points[i] );
        }
        RemoveNullPoints();
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

    private Vector3 loBoundPoint;
    private Vector3 loBoundNormal;
    private Vector3 hiBoundPoint;
    private Vector3 hiBoundNormal;
    private float t;
    public int tesselation = 3;
    private List<float> distances = new List<float>();
    //private List<Vector3> localMinima = new List<Vector3>();
    //private List<Vector3> localDx = new List<Vector3>();
    //private List<Vector3> debugPoints = new List<Vector3>();
    //private List<float> ts = new List<float>();
    private List<Spoint> spoints = new List<Spoint>();
    public Spoint GetNearestSpoint( Vector3 WSPoint )
    {
        tesselation = Max( tesselation, 1 );
        subdivision = Max( subdivision, 1 );
        if (points.Count > 0) {
            //    localMinima.Clear();
            //    localDx.Clear();
            //    ts.Clear();
            spoints.Clear();
            distances.Clear();

            spoints.Add( new Spoint { position = this[0].transform.position, derivative = GetDx( 0 ), tFull = 0, tLocal = 0 } );
            spoints.Add( new Spoint { position = this[points.Count - 1].transform.position, derivative = GetDx( points.Count - 1 ), tFull = 1, tLocal = points.Count - 1 } );

            float res = 1 / (float)subdivision;
            for (int i = 0; i < points.Count - 1; i++) {
                for (float n = i; n < i + 1; n += res) {
                    GetSplineBounds( res, n );
                    if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                        for (float d = n; d < n + res; d += res / (float)tesselation) {
                            GetSplineBounds( res / (float)tesselation, d );

                            if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                                for (float p = d; p < d + (res / (float)tesselation); p += res / (float)(tesselation * tesselation)) {
                                    GetSplineBounds( res / (float)(tesselation * tesselation), p );

                                    if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                                        t = MeasureBetweenPlaneBounds( WSPoint,
                                                                       loBoundPoint,
                                                                       loBoundNormal,
                                                                       hiBoundPoint,
                                                                       hiBoundNormal );
                                        t = Lerp( p, p + res / (float)(tesselation * tesselation * tesselation), t );
                                        //t *= res / (float)(tesselation * tesselation * tesselation);
                                        //t += l;
                                        Spoint spoint = new Spoint {
                                            position = GetPoint( t ),
                                            derivative = GetDx( t ),
                                            tLocal = t,
                                            tFull = t / ((float)points.Count - 1)
                                        };
                                        spoints.Add(
                                            spoint
                                             );
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //localMinima.Add( this[0].transform.position );
            //localMinima.Add( this[points.Count - 1].transform.position );

            //localDx.Add( GetDx( 0 ) );
            //localDx.Add( GetDx( points.Count - 1 ) );

            //ts.Add( 0 );
            //ts.Add( points.Count - 1 );
        }

        for (int i = 0; i < spoints.Count; i++) {
            distances.Add( Vector3.Distance( spoints[i].position, WSPoint ) );
        }

        for (int i = 0; i < spoints.Count; i++) {
            if (i == distances.FindIndex( ( x ) => x == Min( distances ) )) {
                return spoints[i];
            }
        }
        Debug.LogWarning( "Closest spline point not found." );
        return new Spoint();

        void GetSplineBounds( float step, float t )
        {
            loBoundPoint = GetPoint( t );
            loBoundNormal = GetDx( t ).normalized;
            hiBoundPoint = GetPoint( t + step );
            hiBoundNormal = GetDx( t + step ).normalized;
        }
    }
    //public Vector3 GetClosestPoint( Vector3 WSPoint ) => GetClosestPoint( WSPoint, out Vector3 garbage );
    //public Vector3 GetClosestDx( Vector3 WSPoint )
    //{
    //    Vector3 Dx;
    //    GetClosestPoint( WSPoint, out Dx );
    //    return Dx;
    //}
    //public Spoint GetNearestPoint( Vector3 WSPoint, int resolution, int tesselation = 6 )
    //{
    //    Spoint spoint = new Spoint();

    //    return spoint;
    //}
}

public struct Spoint
{
    public Vector3 position;
    public Vector3 derivative;
    public float tLocal;
    public float tFull;
}