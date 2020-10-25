using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mathk;

public class DrawMath : MonoBehaviour
{
    public GameObject debugPoint;
    public GameObject plane2;
    public GameObject splineObj;
    public PathSpline spline;

    public int tesselation = 4;

    private void Awake()
    {
    }
    private List<Vector3> localMinima = new List<Vector3>();
    private List<float> distances = new List<float>();
    private void OnDrawGizmos()
    {
        spline = GetComponent<PathSpline>();

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere( spline.GetNearestSpoint( transform.position ).position, 0.2f );

        DebugSpline();
    }
    private Vector3 loBoundPoint;
    private Vector3 loBoundNormal;
    private Vector3 hiBoundPoint;
    private Vector3 hiBoundNormal;
    private float t;
    private Vector3 result;
    private Ray ray;

    private void DebugSpline()
    {
        localMinima.Clear();
        distances.Clear();
        Vector3 WSPoint = debugPoint.transform.position;
        float step = 1 / (float)spline.subdivision;
        tesselation = spline.tesselation;
        for (int i = 0; i < spline.points.Count - 1; i++) {
            for (float n = i; n < i + 1; n += step) {
                GetSplineBounds( step, n );
                if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                    for (float d = n; d < n + step; d += step / (float)tesselation) {
                        GetSplineBounds( step / (float)tesselation, d );

                        if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                            for (float p = d; p < d + (step / (float)tesselation); p += step / (float)(tesselation * tesselation)) {
                                GetSplineBounds( step / (float)(tesselation * tesselation), p );

                                if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                                    for (float l = p; l < p + (step / (float)(tesselation * tesselation)); l += step / (float)(tesselation * tesselation * tesselation)) {
                                        GetSplineBounds( step / (float)(tesselation * tesselation * tesselation), l );

                                        if (IsAbovePlane( WSPoint, loBoundPoint, loBoundNormal ) && IsAbovePlane( WSPoint, hiBoundPoint, -hiBoundNormal )) {
                                            t = MeasureBetweenPlaneBounds( WSPoint,
                                                                           loBoundPoint,
                                                                           loBoundNormal,
                                                                           hiBoundPoint,
                                                                           hiBoundNormal );

                                            localMinima.Add( Vector3.LerpUnclamped( loBoundPoint, hiBoundPoint, t ) );
                                        }
                                        DrawSplineSegment();
                                    }
                                }
                                else {
                                    DrawSplineSegment();
                                }
                            }
                        }
                        else {
                            DrawSplineSegment();
                        }
                    }
                }
                else {
                    DrawSplineSegment();
                }
            }
        }
        localMinima.Add( spline[0].transform.position );
        localMinima.Add( spline[spline.points.Count - 1].transform.position );
        for (int i = 0; i < localMinima.Count; i++) {
            distances.Add( Vector3.Distance( localMinima[i], transform.position ) );
        }
        for (int i = 0; i < localMinima.Count; i++) {
            if (i == distances.FindIndex( ( x ) => x == Min( distances ) )) {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere( localMinima[i], 0.15f );
            }
            else {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere( localMinima[i], 0.1f );
            }
        }
    }
    private void GetSplineBounds( float step, float t )
    {
        loBoundPoint = spline.GetPoint( t );
        loBoundNormal = spline.GetDx( t ).normalized;
        hiBoundPoint = spline.GetPoint( t + step );
        hiBoundNormal = spline.GetDx( t + step ).normalized;
    }
    private void DrawSplineSegment()
    {
        Gizmos.color = new Color( 1, 1, 1, 0.2f );
        DrawPlane( loBoundPoint, loBoundNormal );
        DrawPlane( hiBoundPoint, hiBoundNormal );
        Gizmos.color = new Color( 1, 1, 1, 1f );
        Gizmos.DrawLine( loBoundPoint, hiBoundPoint );
    }

    private void DrawPlane( Vector3 origin, Vector3 normal )
    {
        int subdivisions = 4;
        for (float i = 0; i < 1; i += 1 / (float)subdivisions) {
            Quaternion rot = Quaternion.LookRotation( normal, Vector3.up );
            Vector3 spoke = Quaternion.AngleAxis( i * 360, rot.Forward() ) * rot.Right() * 0.5f;

            DrawCircle( origin, normal, i * 0.5f );
            Gizmos.DrawRay( origin, spoke );
        }
    }
    public void DrawCircle( Vector3 origin, Vector3 normal, float radius )
    {
        float step = 1 / (float)6;
        for (float i = 0; i < 1; i += step) {
            Quaternion rot = Quaternion.LookRotation( normal, Vector3.up );
            Vector3 start = Quaternion.AngleAxis( i * 360, rot.Forward() ) * rot.Right() * radius;
            Vector3 end = Quaternion.AngleAxis( (i + step) * 360, rot.Forward() ) * rot.Right() * radius;
            start += origin;
            end += origin;
            Gizmos.DrawLine( start, end );
        }
    }
}