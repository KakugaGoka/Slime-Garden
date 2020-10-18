using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowPath : MonoBehaviour {

    //Behaviors
    NavMeshAgent m_Agent;
    Behaviors m_Behaviors;
    Action Go;
    Action CanSee;
    NodeState currentState;

    [Header("Main Varaibles")]
    [Tooltip("The Tag you want this entity to look for.")]
    public string tagTarget = "Toy";
    [Tooltip("The list of destinations to travel between while looking for the above object")]
    public Vector3[] destinationPaths;

    private int currentDestination = 0;

    void Start() {
        m_Agent = gameObject.GetComponent<NavMeshAgent>();
        m_Behaviors = gameObject.GetComponent<Behaviors>();

        if (destinationPaths.Length > 0) {
            Go = new Action(m_Behaviors.a_Follow, new object[] { destinationPaths[currentDestination] });
            NodeState testGo = Go.Evaluate();
            Debug.Log(testGo.ToString());
        }
        CanSee = new Action(m_Behaviors.a_CanSeeObject, new object[] { tagTarget });
    }

    void Update() {
        if (CanSee.Evaluate() == NodeState.Success) {
            this.Go = new Action(m_Behaviors.a_Follow, new object[] { GameObject.FindGameObjectWithTag(tagTarget).transform.position});
        } else {
            if (destinationPaths.Length > 0) {
                if (currentState == NodeState.Success || currentState == NodeState.Failure) {
                    currentDestination++;
                    if (currentDestination >= destinationPaths.Length) { currentDestination = 0; }
                    this.Go = new Action(m_Behaviors.a_Follow, new object[] { destinationPaths[currentDestination] });
                }
            }
        }
        currentState = Go.Evaluate();
    }
}
