using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeStationController : MonoBehaviour
{
    public InputField nameField;
    public Button nameButton;

    private SlimeController m_Slime;
    private InteractController m_InteractController;

    void Awake() {
        m_InteractController = GetComponent<InteractController>();
        m_InteractController.onInteract += SetUp;
    }

    private void SetUp(PlayerCharacterController player) {
        if (player.heldItem) {
            m_Slime = player.heldItem.GetComponent<SlimeController>();
            if (m_Slime) {
                OnFound(m_Slime);
            } else {
                OnFail();
            }
        } else {
            OnFail();
        }
    }

    private void OnFail() {
        nameField.text = "No slime detected... Have a nice day!";
        nameField.interactable = false;
        nameButton.interactable = false;
    }

    private void OnFound(SlimeController slime) {
        nameField.placeholder.enabled = false;
        nameField.text = slime.name;
        nameField.interactable = true;
        nameButton.interactable = true;
    }

    public void Rename() {
        if (m_Slime) {
            m_Slime.name = nameField.text;
        }
    }

    public void DeleteSlime() {
        if (m_Slime) {
            Destroy(m_Slime.gameObject);
        }
    }
}
