using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCameraController : MonoBehaviour
{
    public GameObject thingToFollow;

    void Update()
    {
        if (thingToFollow) {
            transform.position = thingToFollow.transform.position + (Vector3.right * 3) + (Vector3.up * 3);
            transform.LookAt(thingToFollow.transform);
        }
    }
}
