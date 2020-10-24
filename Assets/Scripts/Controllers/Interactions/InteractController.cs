using UnityEngine;
using UnityEngine.Events;

public class InteractController : MonoBehaviour {
    [Header("Parameters")]
    [Tooltip("The action that will appear in the interact message")]
    public string interactionMessage = "Interact";
    [Tooltip("The color of the text that will appear in the interact message")]
    public Color messageColor = Color.white;

    public UnityAction<PlayerCharacterController> onInteract;
}
