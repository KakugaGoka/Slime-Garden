﻿using UnityEngine;

public class PlayerInputHandler : MonoBehaviour {
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float lookSensitivity = 1f;
    [Tooltip("Additional sensitivity multiplier for WebGL")]
    public float webglLookSensitivityMultiplier = 0.25f;
    [Tooltip("Limit to consider an input when using a trigger on a controller")]
    public float triggerAxisThreshold = 0.4f;
    [Tooltip("Used to flip the vertical input axis")]
    public bool invertYAxis = true;
    [Tooltip("Used to flip the horizontal input axis")]
    public bool invertXAxis = false;

    PlayerCharacterController m_PlayerCharacterController;
    bool m_FireInputWasHeld;

    private void Start() {
        m_PlayerCharacterController = GetComponent<PlayerCharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //private void LateUpdate() {
    //    m_FireInputWasHeld = GetFireInputHeld();
    //}

    public bool CanProcessInput() {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public Vector3 GetMoveInput() {
        if (CanProcessInput()) {
            Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f, Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

            // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
            move = Vector3.ClampMagnitude(move, 1);

            return move;
        }

        return Vector3.zero;
    }

    public float GetLookInputsHorizontal() {
        return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameHorizontal, invertXAxis);
    }

    public float GetLookInputsVertical() {
        return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameVertical, invertYAxis);
    }

    public bool GetJumpInputDown() {
        if (CanProcessInput()) {
            return Input.GetButtonDown(GameConstants.k_ButtonNameJump);
        }

        return false;
    }

    public bool GetJumpInputHeld() {
        if (CanProcessInput()) {
            return Input.GetButton(GameConstants.k_ButtonNameJump);
        }

        return false;
    }
    public bool GetInteractInputDown() {

        if (CanProcessInput()) {
            return Input.GetButtonDown(GameConstants.k_ButtonNameInteract);
        }

        return false;
    }

    public bool GetPauseInputDown() {

        if (CanProcessInput()) {
            return Input.GetButtonDown(GameConstants.k_ButtonNameCancel);
        }

        return false;
    }

    public bool GetDropInputDown() {

        if (CanProcessInput()) {
            return Input.GetButtonDown(GameConstants.k_ButtonNameDrop);
        }

        return false;
    }

    public bool GetSprintInputHeld() {
        if (CanProcessInput()) {
            return Input.GetButton(GameConstants.k_ButtonNameSprint);
        }

        return false;
    }

    public bool GetCrouchInputDown() {
        if (CanProcessInput()) {
            return Input.GetButtonDown(GameConstants.k_ButtonNameCrouch);
        }

        return false;
    }

    public bool GetCrouchInputReleased() {
        if (CanProcessInput()) {
            return Input.GetButtonUp(GameConstants.k_ButtonNameCrouch);
        }

        return false;
    }

    public int GetRollItemInput() {
        if (CanProcessInput()) {

            string axisName = GameConstants.k_ButtonNameRoll;

            if (Input.GetAxis(axisName) > 0f)
                return -1;
            else if (Input.GetAxis(axisName) < 0f)
                return 1;
        }

        return 0;
    }

    public int GetRotateItemInput() {
        if (CanProcessInput()) {
            if (Input.GetButton(GameConstants.k_ButtonNameRotateLeft))
                return -1;
            else if (Input.GetButton(GameConstants.k_ButtonNameRotateRight))
                return 1;
        }
        return 0;
    }

    public int GetSelectWeaponInput() {
        if (CanProcessInput()) {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                return 1;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                return 2;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                return 3;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                return 4;
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                return 5;
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                return 6;
            else
                return 0;
        }

        return 0;
    }

    float GetMouseOrStickLookAxis(string mouseInputName, bool invert) {
        if (CanProcessInput()) {
            // Check if this look input is coming from the mouse
            float i = Input.GetAxisRaw(mouseInputName);

            // handle inverting vertical input
            if (invert)
                i *= -1f;

            // apply sensitivity multiplier
            i *= lookSensitivity;

            // reduce mouse input amount to be equivalent to stick movement
            i *= 0.01f;

            return i;
        }

        return 0f;
    }
}
