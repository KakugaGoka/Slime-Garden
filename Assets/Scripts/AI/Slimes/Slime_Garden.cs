using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Unity.Mathematics.math;

public class Slime_Garden : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;

    private Vector3 targetLocation;
    private GameObject lookAtObj;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        lookAtObj = GameObject.FindGameObjectWithTag( "Player" );
    }

    private void Update()
    {
        Quaternion targetRot = Quaternion.LookRotation( transform.forward, Vector3.up );
    }

    private void FixedUpdate()
    {
        Vector3 lookDir = (lookAtObj.transform.position - transform.position).normalized;

        Vector3 upTorque = cross( transform.up, Vector3.up ) * remap( -1, 1, 0, 1, dot( transform.up, Vector3.up ) );
        if (upTorque != Vector3.zero) {
            upTorque = normalize( upTorque );
        }
        rb.AddTorque( upTorque );
        //rb.AddTorque( Vector3.Cross( transform.up, Vector3.up ) );
        rb.AddTorque( Vector3.Cross( transform.forward, lookDir ) );
    }

    private bool jumping = true;

    private IEnumerable HopCoroutine( Vector3 target )
    {
        if (!jumping) {
        }
        yield return new WaitForEndOfFrame();
    }

    private void OnCollisionEnter( Collision collision )
    {
        for (int i = 0; i < collision.contactCount; i++) {
            if (true) {
            }
            //collision.contacts
        }
    }

    private Vector3 WanderPoint()
    {
        Vector3 wanderPoint = Random.insideUnitCircle * 10;
        NavMesh.SamplePosition( wanderPoint, out NavMeshHit hit, 10, 0 );
        return hit.position;
    }
}