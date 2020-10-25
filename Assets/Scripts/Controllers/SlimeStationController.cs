using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeStationController : MonoBehaviour
{
    public InputField nameField;
    public Button nameButton;
    public Button goodbyeButton;
    public Button raceButton;

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
        nameField.text = "No slime detected";
        nameField.interactable = false;
        nameButton.interactable = false;
        goodbyeButton.interactable = false;
        raceButton.interactable = false;
    }

    private void OnFound(SlimeController slime) {
        nameField.text = slime.slimeName;
        nameField.interactable = true;
        nameButton.interactable = true;
        goodbyeButton.interactable = true;
        raceButton.interactable = true;
    }

    public void Rename() {
        if (m_Slime) {
            m_Slime.slimeName = nameField.text;
        }
    }

    public void DeleteSlime() {
        if (m_Slime) {
            Destroy(m_Slime.gameObject);
        }
    }
}
