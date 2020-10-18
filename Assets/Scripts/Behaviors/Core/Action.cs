using System;
using UnityEngine;
using System.Collections;

/* Method signature for the action. */
public delegate NodeState ActionDelegate(params object[] parameters);

public class Action : Node {

    /* The delegate that is called to evaluate this node */
    private ActionDelegate m_Action;
    private object[] m_Parameters;

    /* Because this node contains no logic itself, 
     * the logic must be passed in in the form of  
     * a delegate. As the signature states, the action 
     * needs to return a NodeStates enum */
    public Action(ActionDelegate action, params object[] parameters) {
        m_Action = action;
        m_Parameters = parameters;
    }

    /* Evaluates the node using the passed in delegate and  
     * reports the resulting state as appropriate */
    public override NodeState Evaluate() {
        switch (m_Action(m_Parameters)) {
            case NodeState.Success:
                m_nodeState = NodeState.Success;
                return m_nodeState;
            case NodeState.Failure:
                m_nodeState = NodeState.Failure;
                return m_nodeState;
            case NodeState.Running:
                m_nodeState = NodeState.Running;
                return m_nodeState;
            default:
                m_nodeState = NodeState.Failure;
                return m_nodeState;
        }
    }
}