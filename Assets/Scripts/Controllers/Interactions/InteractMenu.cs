using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractController))]
public class InteractMenu : MonoBehaviour
{
    public Canvas menuToOpen;
    public Canvas menuToClose;

    private InteractController m_InteractController;

    void Start()
    {
        m_InteractController = GetComponent<InteractController>();

        m_InteractController.onInteract += OpenMenu;
    }

    void Update()
    {
        
    }

    public void OpenMenu(PlayerCharacterController player) {
        menuToClose.gameObject.SetActive(false);
        menuToOpen.gameObject.SetActive(true);
        SaveLoadManager.isMenuOpen = true;
        GameFlowManager.paused = true;
        Cursor.visible = true;
    }

    public void CloseMenu() {
        menuToClose.gameObject.SetActive(true);
        menuToOpen.gameObject.SetActive(false);
        SaveLoadManager.isMenuOpen = false;
        GameFlowManager.paused = false;
    }
}
