using UnityEngine;
using System.Collections;

public class Inverter : Node {
    /* Child node to evaluate */
    private Node m_node;

    public Node node {
        get { return m_node; }
    }

    /* The constructor requires the child node that this inverter decorator 
     * wraps*/
    public Inverter(Node node) {
        m_node = node;
    }

    /* Reports a success if the child fails and 
     * a failure if the child succeeds. Running will report 
     * as running */
    public override NodeState Evaluate() {
        switch (m_node.Evaluate()) {
            case NodeState.Failure:
                m_nodeState = NodeState.Success;
                return m_nodeState;
            case NodeState.Success:
                m_nodeState = NodeState.Failure;
                return m_nodeState;
            case NodeState.Running:
                m_nodeState = NodeState.Running;
                return m_nodeState;
        }
        m_nodeState = NodeState.Success;
        return m_nodeState;
    }
}