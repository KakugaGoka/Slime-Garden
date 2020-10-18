using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sequence : Node {
    /** Children nodes that belong to this sequence */
    private List<Node> m_nodes = new List<Node>();

    /** Must provide an initial set of children nodes to work */
    public Sequence(List<Node> nodes) {
        m_nodes = nodes;
    }

    /* If any child node returns a failure, the entire node fails. Whence all  
     * nodes return a success, the node reports a success. */
    public override NodeState Evaluate() {
        bool anyChildRunning = false;

        foreach (Node node in m_nodes) {
            switch (node.Evaluate()) {
                case NodeState.Failure:
                    m_nodeState = NodeState.Failure;
                    return m_nodeState;
                case NodeState.Success:
                    continue;
                case NodeState.Running:
                    anyChildRunning = true;
                    continue;
                default:
                    m_nodeState = NodeState.Success;
                    return m_nodeState;
            }
        }
        m_nodeState = anyChildRunning ? NodeState.Running: NodeState.Success;
        return m_nodeState;
    }
}