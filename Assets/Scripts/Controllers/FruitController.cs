using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( InteractHold ) )]
[RequireComponent( typeof( EdibleController ) )]
public class FruitController : MonoBehaviour
{
    private Rigidbody m_RigidBody;
    private EdibleController m_EdibleController;

    public float currentAge = 0;
    public float maxAge = 600;
    public float satiation = 1;
    public float hopping;
    public float rolling;
    public float floating;
    public float range;

    [HideInInspector]
    public string prefabName;

    private void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_EdibleController = GetComponent<EdibleController>();

        m_EdibleController.onEat += Eat;
    }

    private void Update()
    {
        currentAge += Time.deltaTime;
        if (currentAge >= maxAge) {
            Destroy( gameObject );
        }
    }

    private void OnCollisionEnter( Collision collision )
    {
        transform.parent = null;
        if (m_RigidBody != null) {
            m_RigidBody.useGravity = true;
        }
    }

    public void Eat( SlimeController slime )
    {
        Destroy( gameObject );
    }
}