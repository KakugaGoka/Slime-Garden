using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionController : MonoBehaviour
{
    [Range(0, 30)]
    public float viewDistance;
    [Range(0, 360)]
    public float viewAngle;
    public Camera eyes;

    void Update() {
        if (eyes.fieldOfView != viewAngle) {
            eyes.fieldOfView = viewAngle;
        }
    }

    public bool CanSee(Renderer target) {
        return eyes.VisibleFromCamera(target) && Vector3.Distance(target.transform.position, gameObject.transform.position) <= viewDistance;
    }
}
