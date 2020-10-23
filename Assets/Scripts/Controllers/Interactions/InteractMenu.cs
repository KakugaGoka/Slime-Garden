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

    public void OpenMenu(PlayerCharacterController player) {
        if (menuToClose) menuToClose.gameObject.SetActive(false);
        if (menuToOpen) menuToOpen.gameObject.SetActive(true);
        SaveLoadManager.isMenuOpen = true;
        GameFlowManager.paused = true;
        Cursor.visible = true;
    }

    public void OpenSubMenu() {
        if (menuToClose) menuToClose.gameObject.SetActive(false);
        if (menuToOpen) menuToOpen.gameObject.SetActive(true);
        SaveLoadManager.isMenuOpen = true;
        GameFlowManager.paused = true;
        Cursor.visible = true;
    }

    public void CloseMenu() {
        if (menuToClose) menuToClose.gameObject.SetActive(true);
        if (menuToOpen) menuToOpen.gameObject.SetActive(false);
        SaveLoadManager.isMenuOpen = false;
        GameFlowManager.paused = false;
    }

    public void CloseSubMenu() {
        if (menuToClose) menuToClose.gameObject.SetActive(true);
        if (menuToOpen) menuToOpen.gameObject.SetActive(false);
    }
}
