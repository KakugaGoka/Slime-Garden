using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[System.Serializable]
public class PathPoint : MonoBehaviour
{
    private Vector3[] curveHandles = new Vector3[2];
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere( transform.position, 0.1f );
    }

    private void OnDestroy()
    {
        var spline = transform.parent.GetComponent<PathSpline>();
        spline.points.Remove( gameObject );

        spline.RenamePoints();
    }
}