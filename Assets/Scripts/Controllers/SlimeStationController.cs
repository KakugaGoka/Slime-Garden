using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeStationController : MonoBehaviour
{
    public InputField nameField;
    public Button nameButton;
    public Button goodbyeButton;

    private SlimeController m_Slime;
    private InteractController m_InteractController;
    private GameObject m_EmptyObject;
    private PlayerCharacterController m_Player;

    void Awake() {
        m_InteractController = GetComponent<InteractController>();
        m_InteractController.onInteract += SetUp;
    }

    private void SetUp(PlayerCharacterController player) {
        m_Player = player;
        m_EmptyObject = player.satchel.emptyObject;

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
    }

    private void OnFound(SlimeController slime) {
        nameField.text = slime.slimeName;
        nameField.interactable = true;
        nameButton.interactable = true;
        goodbyeButton.interactable = true;
    }

    public void Rename() {
        if (m_Slime) {
            m_Slime.slimeName = nameField.text;
        }
    }

    public void DeleteSlime() {
        if (m_Slime) {
            Destroy(m_Slime.gameObject);
            Instantiate(m_EmptyObject, m_Player.heldObjectLocation).transform.SetAsFirstSibling();
        }
    }
}
