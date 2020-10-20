using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractHold))]
[RequireComponent(typeof(EdibleController))]
public class FruitController : MonoBehaviour {
    private Rigidbody m_RigidBody;
    private EdibleController m_EdibleController;

    public float currentAge = 0;
    public float maxAge = 600;
    public int satiation = 1;

    [HideInInspector]
    public string prefabName;

    void Start() {
        m_RigidBody = GetComponent<Rigidbody>();
        m_EdibleController = GetComponent<EdibleController>();

        m_EdibleController.onEat += Eat;
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

    private void Eat(Slime slime) {
        Debug.Log("YOU ATE THE FRUIT!!!");
    }
}
