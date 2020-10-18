using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Behaviors: MonoBehaviour {
    // Selectors


    // Sequences

    // Actions
    /// <summary>
    /// Set the NavMeshAgents Destination to a Vector3 passed in as the first index of parameters when action is evaluated.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public NodeState a_Follow(params object[] parameters) {
        if (parameters.Length < 1) { return NodeState.Failure; }
        Vector3 targetVector = (Vector3)parameters[0];
        if (targetVector == null) { return NodeState.Failure; }
        NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null) { return NodeState.Failure; }
        if (navMeshAgent.destination != targetVector) { navMeshAgent.destination = targetVector; }
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;    
            return NodeState.Success; 
        }
        navMeshAgent.isStopped = false;
        return NodeState.Running;
    }

    public NodeState a_Wander(params object[] parameters) {
        NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null) { return NodeState.Failure; }

        Vector3 targetVector = navMeshAgent.destination;
        NavMeshHit hit = new NavMeshHit();
        if (NavMesh.SamplePosition(navMeshAgent.transform.position, out hit, 10f, -1)) {
            targetVector = hit.position;
        }

        if (navMeshAgent.destination != targetVector) { 
            navMeshAgent.destination = targetVector; 
        
        }
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
            return NodeState.Success;
        
        }
        navMeshAgent.isStopped = false;
        return NodeState.Running;
    }

    public NodeState a_CanSeeObject(params object[] parameters) {
        if (parameters.Length < 1) { return NodeState.Failure; }
        VisionController vision = gameObject.GetComponent<VisionController>();
        if (vision == null) { return NodeState.Failure; }
        string tag = (string)parameters[0];
        List<GameObject> targets = GameObject.FindGameObjectsWithTag(tag).ToList();
        if (targets == null || targets.Count == 0) { return NodeState.Failure; }
        List<Renderer> targetRenderers = new List<Renderer>();
        for (int i = 0; i < targets.Count; i++) {
            if (Vector3.Distance(targets[i].transform.position, gameObject.transform.position) > vision.viewDistance) {
                targets.Remove(targets[i]);
                i--;
            } else {
                Renderer renderer = targets[i].GetComponent<Renderer>();
                if (renderer != null) {
                    targetRenderers.Add(renderer);
                }
                Renderer[] renderers = targets[i].GetComponentsInChildren<Renderer>();
                foreach(Renderer rend in renderers) {
                    targetRenderers.Add(rend);
                }
            }
        }
        if (targetRenderers.Count < 1) { return NodeState.Failure; }
        foreach (Renderer renderer in targetRenderers) {
            if (vision.CanSee(renderer)) {
                return NodeState.Success;
            }
        }
        return NodeState.Failure;
    }
}
