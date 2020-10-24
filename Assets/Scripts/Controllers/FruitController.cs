using System;
using UnityEngine;

[RequireComponent( typeof( InteractHold ) )]
[RequireComponent( typeof( EdibleController ) )]
[RequireComponent(typeof(AgeController))]
[Serializable]
public class FruitController : MainController {
    private Rigidbody m_RigidBody;
    private EdibleController m_EdibleController;
    private AgeController m_AgeController;

    public float satiation = 30;
    public float hopping = 0;
    public float rolling = 0;
    public float floating = 0;
    public float range = 0;
    public bool hasFallen = false;
    public GameObject tree;

    [HideInInspector]
    public string prefabName;

    private void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_EdibleController = GetComponent<EdibleController>();
        m_AgeController = GetComponent<AgeController>();

        m_EdibleController.onEat += Eat;
    }

    private void OnCollisionEnter( Collision collision )
    {
        if (!hasFallen) {
            m_AgeController.customAging = true;
            transform.parent = null;
            if (m_RigidBody != null) {
                m_RigidBody.useGravity = true;
                hasFallen = true;
            }
        }
    }

    public void Eat( SlimeController slime )
    {
        slime.hopping += hopping;
        slime.rolling += rolling;
        slime.floating += floating;
        slime.range += range;
        slime.hunger -= satiation;
    }
}