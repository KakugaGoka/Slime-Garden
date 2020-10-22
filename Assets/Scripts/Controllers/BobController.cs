using UnityEngine;
using System;

public class BobController : MonoBehaviour {
    float startingYPosition;

    public float amplitude = 1; 

    void Start() {
        this.startingYPosition = this.transform.position.y;
    }

    void Update() {
        transform.position = new Vector3(transform.position.x, startingYPosition + ((float)Math.Sin(Time.time) * amplitude), transform.position.z);
    }
}