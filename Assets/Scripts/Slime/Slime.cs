using UnityEngine;
using UnityEngine.AI;
using static Unity.Mathematics.math;

[RequireComponent( typeof( Rigidbody ) )]
[System.Serializable]
public class Slime : MonoBehaviour
{
    private Rigidbody rb;

    private float maxJumpForce;
    private GameObject player;

    private Vector3 targetPosition;

    public float jumpTimer = 5;
    public float wanderRange = 5;
    public float jumpForce = 100;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        player = GameObject.FindGameObjectWithTag( "Player" );
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        float distance = Vector3.Distance( gameObject.Pos(), targetPosition );

        if (distance < 1 || targetPosition == Vector3.zero) {
            targetPosition = FindWanderPoint();
        }
        if (timer < 0) {
            if (Physics.Raycast( gameObject.Pos(), Vector3.down, out RaycastHit hit, 0.6f, -1 )) {
                Hop( targetPosition );
            }
            timer = jumpTimer;
        }

        Turn();
    }

    private void OnCollisionEnter( Collision collision )
    {
        for (int i = 0; i < collision.contactCount; i++) {
            if (collision.relativeVelocity.magnitude > 0.4f && collision.contacts[i].normal.y > 0.8f) {
                rb.angularVelocity *= 0f;
                break;
            }
        }
    }

    private float timer;

    private Vector3 FindWanderPoint()
    {
        Vector3 point = Random.insideUnitSphere * wanderRange + gameObject.Pos();

        if (NavMesh.SamplePosition( point, out NavMeshHit hit, 10, -1 )) {
            return hit.position;
        }
        return Vector3.zero;
    }

    private void Turn()
    {
        Vector3 lookDir = gameObject.lookAt( targetPosition );
        rb.AddTorque( cross( transform.forward, lookDir ) );
        rb.AddTorque( cross( transform.up, Vector3.up ) );
    }

    private void Hop( Vector3 target )
    {
        Vector3 jumpDir = gameObject.lookAt( target );
        jumpDir = Vector3.ProjectOnPlane( jumpDir, Vector3.up ).normalized;
        Quaternion jumpRot = Quaternion.LookRotation( jumpDir );

        Vector3 jumpVector = normalize( float3( 0, 1, 1 ) );
        jumpVector = jumpRot * jumpVector;

        rb.AddForce( jumpVector * jumpForce );
    }
}