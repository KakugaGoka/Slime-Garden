using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractHold))]
public class FruitController : MonoBehaviour {
    private Rigidbody m_RigidBody;
    private float age = 0;

    public float currentAge = 0;
    public float maxAge = 600;

    void Start() {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void Update() {
        currentAge += Time.deltaTime;
        if (currentAge >= maxAge) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        transform.parent = null;
        if (m_RigidBody != null) {
            m_RigidBody.useGravity = true;
        }
    }
}
