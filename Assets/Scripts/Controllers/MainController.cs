using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class MainController : MonoBehaviour
{
    [HideInInspector]
    public float PosX;
    [HideInInspector]
    public float PosY;
    [HideInInspector]
    public float PosZ;

    [HideInInspector]
    public float RotX;
    [HideInInspector]
    public float RotY;
    [HideInInspector]
    public float RotZ;

    public void GetTransformData() {
        this.PosX = gameObject.transform.position.x;
        this.PosY = gameObject.transform.position.y;
        this.PosZ = gameObject.transform.position.z;

        this.RotX = gameObject.transform.rotation.eulerAngles.x;
        this.RotY = gameObject.transform.rotation.eulerAngles.y;
        this.RotZ = gameObject.transform.rotation.eulerAngles.z;
    }
}
