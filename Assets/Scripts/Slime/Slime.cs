using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.math;

[RequireComponent( typeof( Rigidbody ) )]
[System.Serializable]
public class Slime : MonoBehaviour
{
    private Rigidbody rb;

    private float maxJumpForce;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 jumpTarget = float3( 0, 0, 1 );

        if (Input.GetKeyDown( ";" )) {
            Hop( jumpTarget );
        }
    }

    private void Hop( Vector3 target )
    {
    }
}