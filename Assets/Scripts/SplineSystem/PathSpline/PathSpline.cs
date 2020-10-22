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

    public BezierSpline spline = new BezierSpline();
    private void Awake()
    {
    }
    private void OnEnable()
    {
    }

    private void OnDrawGizmos()
    {
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