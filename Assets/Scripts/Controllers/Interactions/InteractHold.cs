using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractController))]
public class InteractHold : MonoBehaviour {

    InteractController m_InteractController;
    Rigidbody m_Rigidbody;
    bool isHeld = false;

    public Vector3 holdPosition = new Vector3(0, 0, 0);
    public int value = 5;

    public UnityAction<PlayerCharacterController> onDrop;

    private void Awake() {
        m_InteractController = GetComponent<InteractController>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_InteractController.onInteract += OnInteract;
        this.onDrop += OnDrop;
    }

    private void LateUpdate() {
        if (gameObject.transform.localPosition != holdPosition && isHeld) {
            gameObject.transform.localPosition = holdPosition;
        }
    }

    public void OnInteract(PlayerCharacterController player) {
        if (m_Rigidbody != null) {
            m_Rigidbody.useGravity = false;
            m_Rigidbody.detectCollisions = false;
        }
        this.gameObject.transform.parent = player.heldObjectLocation;
        this.gameObject.transform.localPosition = holdPosition;
        isHeld = true;
        player.isHolding = true;
        player.heldItem = this;
    }

    public void OnDrop(PlayerCharacterController player) {
        if (m_Rigidbody != null) {
            m_Rigidbody.useGravity = true;
            m_Rigidbody.detectCollisions = true;
            m_Rigidbody.velocity = player.playerCamera.transform.forward * 15 + player.characterVelocity;
        }
        this.gameObject.transform.parent = null;
        isHeld = false;
        player.isHolding = false;
        player.heldItem = null;
    }
}
