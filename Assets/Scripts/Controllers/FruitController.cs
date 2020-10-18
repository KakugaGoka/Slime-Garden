using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractHold))]
public class FruitController : MonoBehaviour {
    private Rigidbody m_RigidBody;

    void Start() {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) {
        transform.parent = null;
        if (m_RigidBody != null) {
            m_RigidBody.useGravity = true;
        }
    }
}
