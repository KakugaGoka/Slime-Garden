using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Node {

    /* Delegate that returns the state of the node.*/
    public delegate NodeState NodeReturn();

    /* The current state of the node */
    protected NodeState m_nodeState;

    public NodeState nodeState {
        get { return m_nodeState; }
    }

    /* The constructor for the node */
    public Node() { }

    /* Implementing classes use this method to evaluate the desired set of conditions */
    public abstract NodeState Evaluate();

}
