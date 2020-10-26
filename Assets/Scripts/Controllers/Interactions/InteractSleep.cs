using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractController))]
public class InteractSleep : MonoBehaviour
{
    private InteractController m_Interact;
    void Start()
    {
        m_Interact = GetComponent<InteractController>();

        m_Interact.onInteract += Sleep;
    }

    public void Sleep(PlayerCharacterController player) {
        player.playerCamera.clearFlags = CameraClearFlags.Color;
        player.playerCamera.backgroundColor = Color.black;
        player.playerCamera.cullingMask = -2;
        player.enabled = false;
        TimeManager.main.gameTimeScale = 100;
        StartCoroutine(Sleeping(player));
    }

    IEnumerator Sleeping(PlayerCharacterController player) {
        while (TimeManager.main.currentHour != 6) {
            Debug.Log(TimeManager.main.currentHour);
            yield return null;
        }
        player.playerCamera.clearFlags = CameraClearFlags.Skybox;
        player.playerCamera.cullingMask = -1;
        TimeManager.main.gameTimeScale = 1;
        player.enabled = true;
    }
}
