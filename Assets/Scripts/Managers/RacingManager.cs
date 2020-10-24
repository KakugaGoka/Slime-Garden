using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingManager : MonoBehaviour
{
    private SlimeController[] slimes;
    private void Start()
    {
        slimes = FindObjectsOfType<SlimeController>();
        for (int i = 0; i < slimes.Length; i++) {
            slimes[i].activity = SlimeController.Activity.Racing;
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}