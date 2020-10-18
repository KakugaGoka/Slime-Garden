using UnityEngine;
using UnityEngine.AI;

public class Slime_Garden : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;

    private Vector3 targetLocation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
    }

    private Vector3 WanderPoint()
    {
        Vector3 wanderPoint = Random.insideUnitCircle * 10;
        NavMesh.SamplePosition(wanderPoint, out NavMeshHit hit, 10, 0);
        return hit.position;
    }
}