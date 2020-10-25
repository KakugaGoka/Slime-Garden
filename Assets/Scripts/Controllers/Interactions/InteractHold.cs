using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractController))]
public class InteractHold : MonoBehaviour {

    InteractController m_InteractController;
    PlayerCharacterController m_Player;
    Rigidbody m_Rigidbody;

    public bool canBeStowed = true;
    public bool isHeld = false;
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
        if (m_Player) {
            if (transform.localRotation.eulerAngles != m_Player.heldItemRotation) {
                transform.localRotation = Quaternion.Euler(m_Player.heldItemRotation);
            }
        }
    }

    public void OnInteract(PlayerCharacterController player) {
        m_Player = player;
        if (m_Rigidbody) {
            m_Rigidbody.useGravity = false;
        }
        this.gameObject.transform.parent = player.heldObjectLocation;
        if (!player.heldItem) {
            Destroy(player.heldObjectLocation.GetChild(0).gameObject);
        }
        this.gameObject.transform.SetAsFirstSibling();
        this.gameObject.transform.localPosition = holdPosition;
        isHeld = true;
        player.isHolding = true;
        player.heldItem = this;
        player.heldItemRotation = transform.localRotation.eulerAngles;
    }

    public void OnDrop(PlayerCharacterController player) {
        m_Player = null;
        if (m_Rigidbody) {
            m_Rigidbody.useGravity = true;
            int rate = 1;
            if (Input.GetButton(GameConstants.k_ButtonNameSprint)) {
                rate = 15;
            }
            Vector3 velocity = player.playerCamera.transform.forward * rate + player.characterVelocity;
            m_Rigidbody.velocity = velocity;
        }
        this.gameObject.transform.parent = null;
        isHeld = false;
        player.isHolding = false;
        player.heldItem = null;
    }
}
