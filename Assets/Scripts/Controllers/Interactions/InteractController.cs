using UnityEngine;
using UnityEngine.Events;

public class InteractController : MonoBehaviour {
    [Header("Parameters")]
    [Tooltip("The action that will appear in the interact message")]
    public string interactionMessage = "Press 'E' to interact";
    [Tooltip("The color of the text that will appear in the interact message")]
    public ColorConstant messageColor = ColorConstant.White;

    public UnityAction<PlayerCharacterController> onInteract;

    public Color interactionColor { get; private set; }

    [HideInInspector]
    public bool interactable = false;

    private void Update() {
        if (GameConstants.ColorFromConstant(messageColor) != interactionColor) {
            interactionColor = GameConstants.ColorFromConstant(messageColor);
        }
    }
}
