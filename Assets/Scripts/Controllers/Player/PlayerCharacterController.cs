﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent( typeof( CharacterController ), typeof( PlayerInputHandler ), typeof( AudioSource ) )]
public class PlayerCharacterController : MonoBehaviour
{
    [Header( "References" )]
    [Tooltip( "Reference to the main camera used for the player" )]
    public Camera playerCamera;

    [Tooltip( "Audio source for footsteps, jump, etc..." )]
    public AudioSource audioSource;

    [Tooltip( "The empty object to parent held items under." )]
    public Transform heldObjectLocation;

    [Tooltip("The text box that contains the player's current money.")]
    public Text wallet;

    [Tooltip("The text box that contains the player's current money.")]
    public SlimeMenuController slimeMenu;

    [Header( "General" )]
    [Tooltip( "The current amount of wealth had by the player." )]
    public int wealth;

    [Tooltip( "Force applied downward when in the air" )]
    public float gravityDownForce = 20f;

    [Tooltip( "Physic layers checked to consider the player grounded" )]
    public LayerMask groundCheckLayers = -1;

    [Tooltip( "distance from the bottom of the character controller capsule to test for grounded" )]
    public float groundCheckDistance = 0.05f;

    [Header( "Movement" )]
    [Tooltip( "Max movement speed when grounded (when not sprinting)" )]
    public float maxSpeedOnGround = 10f;

    [Tooltip( "Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite" )]
    public float movementSharpnessOnGround = 15;

    [Tooltip( "Max movement speed when crouching" )]
    [Range( 0, 1 )]
    public float maxSpeedCrouchedRatio = 0.5f;

    [Tooltip( "Max movement speed when not grounded" )]
    public float maxSpeedInAir = 10f;

    [Tooltip( "Acceleration speed when in the air" )]
    public float accelerationSpeedInAir = 25f;

    [Tooltip( "Multiplicator for the sprint speed (based on grounded speed)" )]
    public float sprintSpeedModifier = 2f;

    [Header( "Rotation" )]
    [Tooltip( "Rotation speed for moving the camera" )]
    public float rotationSpeed = 200f;

    [Header( "Jump" )]
    [Tooltip( "Force applied upward when jumping" )]
    public float jumpForce = 9f;

    [Header( "Stance" )]
    [Tooltip( "Ratio (0-1) of the character height where the camera will be at" )]
    public float cameraHeightRatio = 0.9f;

    [Tooltip( "Height of character when standing" )]
    public float capsuleHeightStanding = 1.8f;

    [Tooltip( "Height of character when crouching" )]
    public float capsuleHeightCrouching = 0.9f;

    [Tooltip( "Speed of crouching transitions" )]
    public float crouchingSharpness = 10f;

    [Header( "Audio" )]
    [Tooltip( "Amount of footstep sounds played when moving one meter" )]
    public float footstepSFXFrequency = 1f;

    [Tooltip( "Amount of footstep sounds played when moving one meter while sprinting" )]
    public float footstepSFXFrequencyWhileSprinting = 1f;

    [Tooltip( "Sound played for footsteps" )]
    public AudioClip footstepSFX;

    [Tooltip( "Sound played when jumping" )]
    public AudioClip jumpSFX;

    [Tooltip( "Sound played when landing" )]
    public AudioClip landSFX;

    [Tooltip( "Sound played when taking damage froma fall" )]
    public AudioClip fallDamageSFX;

    [Header( "Interaction" )]
    [Tooltip( "The distance that the player can reach to inteact with objects" )]
    public float playerReach = 3.5f;

    public UnityAction<bool> onStanceChanged;

    public Vector3 characterVelocity { get; set; }
    public bool isGrounded { get; private set; }
    public bool hasJumpedThisFrame { get; private set; }
    public bool isDead { get; private set; }
    public bool isCrouching { get; private set; }
    public bool isHolding { get; set; }
    public bool isSprinting { get; set; }
    public InteractHold heldItem;

    [HideInInspector]
    public SatchelController satchel;

    private PlayerInputHandler m_InputHandler;
    private CharacterController m_Controller;
    private InteractController m_Interactable;
    private Text m_InteractMessage;
    private Text m_DropMessage;
    private Vector3 m_GroundNormal;
    private Vector3 m_LatestImpactSpeed;
    private float m_LastTimeJumped = 0f;
    private float m_CameraVerticalAngle = 0f;
    private float m_footstepDistanceCounter;
    private float m_TargetCharacterHeight;

    private const float k_JumpGroundingPreventionTime = 0.2f;
    private const float k_GroundCheckDistanceInAir = 0.07f;

    private void Start()
    {
        // fetch components on the same gameObject
        m_Controller = GetComponent<CharacterController>();

        m_InputHandler = GetComponent<PlayerInputHandler>();

        satchel = GetComponent<SatchelController>();

        m_InteractMessage = GameObject.FindGameObjectWithTag( "InteractMessage" ).GetComponent<Text>();
        m_DropMessage = GameObject.FindGameObjectWithTag( "DropMessage" ).GetComponent<Text>();

        m_Controller.enableOverlapRecovery = true;

        // force the crouch state to false when starting
        SetCrouchingState( false, true );
        UpdateCharacterHeight( true );
    }

    private void Update()
    {
        if (m_InputHandler.GetPauseInputDown()) {
            GameFlowManager.Pause();
        }

        if (GameFlowManager.paused) { return; } // don't allow updates during pause.

        wealth = Mathf.Clamp( wealth, 0, 999999999 );
        wallet.text = "Wealth: " + wealth.ToString() + "©";

        hasJumpedThisFrame = false;

        bool wasGrounded = isGrounded;
        GroundCheck();

        // landing
        if (isGrounded && !wasGrounded) {
            // land SFX
            if (landSFX) {
                audioSource.PlayOneShot( landSFX );
            }
        }

        // crouching
        if (m_InputHandler.GetCrouchInputDown()) {
            SetCrouchingState( !isCrouching, false );
        }

        UpdateCharacterHeight( false );

        HandleCharacterMovement();

        m_DropMessage.text = isHolding ? isSprinting ? "Throw" : "Drop" : "";

    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown("=")) {
            PlayerPrefs.DeleteAll();
            Debug.LogWarning("Deleted all save data!");
        }
        if (!heldItem) {
            isHolding = false;
            m_Interactable = null;
            slimeMenu.gameObject.SetActive(false);
        }
        HandleInteractionCheck();
        HandleHeld();
        HandleDropObject();
    }

    private void HandleHeld() {
        if (isHolding) {
            PlantTree();
        }

    }

    private void PlantTree() {
        FruitController fruit = heldItem.GetComponent<FruitController>();
        if (fruit) {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 1000, -1, QueryTriggerInteraction.Collide)) {
                if (hit.distance <= playerReach) {
                    if (hit.collider.gameObject.tag == "FertileGround") {
                        RaycastHit[] hits = Physics.SphereCastAll(hit.point, 2, Vector3.up, 2);
                        bool areaClear = true;
                        foreach (RaycastHit obj in hits) {
                            GameObject game = obj.collider.gameObject;
                            if (game.transform.position.y >= hit.point.y + 0.01) {
                                areaClear = game.tag == "FertileGround";
                            }
                        }
                        if (areaClear) {
                            m_InteractMessage.Set("Plant Tree", Color.white);
                            if (m_InputHandler.GetInteractInputDown()) {
                                Instantiate(fruit.tree, hit.point, Quaternion.identity);
                                Destroy(fruit.gameObject);
                                Instantiate(satchel.emptyObject, heldObjectLocation).transform.SetAsFirstSibling();
                            }
                        }
                    }
                }
            }
        }
    }

    private void HandleInteractionCheck() {
        if (isHolding) {
            SetSlimeMenuData(heldItem.GetComponent<SlimeController>());
        }
        if (Physics.Raycast( playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 1000, -1, QueryTriggerInteraction.Collide )) {
            if (hit.distance <= playerReach) {
                InteractController interactController = hit.collider.GetComponentInParent<InteractController>();
                InteractHold holdController = hit.collider.GetComponentInParent<InteractHold>();
                if (holdController && isHolding) {
                    SetSlimeMenuData(heldItem.GetComponent<SlimeController>());
                    return; 
                }
                if (interactController) {
                    m_Interactable = interactController;
                }
                else {
                    if (m_Interactable) {
                        m_Interactable = null;
                    }
                }
            }
            else {
                m_Interactable = null;
            }
        }
        if (m_InteractMessage) {
            if (m_Interactable) {
                m_InteractMessage.Set( m_Interactable.interactionMessage, m_Interactable.messageColor );
                if (m_InputHandler.GetInteractInputDown()) {
                    m_Interactable.onInteract.Invoke( this );
                }
            }
            else {
                m_InteractMessage.Set( "", Color.white );
            }
        }
    }

    private void HandleDropObject()
    {
        if (isHolding && m_InputHandler.GetDropInputDown() && heldItem != null) {
            heldItem.onDrop.Invoke( this );
        }
    }

    public void SetSlimeMenuData(SlimeController slime) {
        if (!slime) {
            slimeMenu.gameObject.SetActive(false);
            return;
        }
        if (Input.GetKeyDown("[")) {
            SlimeShaderController.ResetAllShaderValues(slime);
            slime.SetInShader();
        }
        slimeMenu.gameObject.SetActive(true);
        slimeMenu.slimeName.text = slime.gameObject.name;
        slimeMenu.hunger.Set(slime.hunger, slime.HungerLimit * 5);
        slimeMenu.hopping.Set(slime.hopping, 100);
        slimeMenu.rolling.Set(slime.rolling, 100);
        slimeMenu.floating.Set(slime.floating, 100);
        slimeMenu.range.Set(slime.range, 100);
    }

    private void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = isGrounded ? (m_Controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

        // reset values before the ground check
        isGrounded = false;
        m_GroundNormal = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime) {
            // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            if (Physics.CapsuleCast( GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere( m_Controller.height ), m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore )) {
                // storing the upward direction for the surface found
                m_GroundNormal = hit.normal;

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot( hit.normal, transform.up ) > 0f &&
                    IsNormalUnderSlopeLimit( m_GroundNormal )) {
                    isGrounded = true;

                    // handle snapping to the ground
                    if (hit.distance > m_Controller.skinWidth) {
                        m_Controller.Move( Vector3.down * hit.distance );
                    }
                }
            }
        }
    }

    private void HandleCharacterMovement()
    {
        // horizontal character rotation
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate( new Vector3( 0f, (m_InputHandler.GetLookInputsHorizontal() * rotationSpeed), 0f ), Space.Self );
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * rotationSpeed;

            // limit the camera's vertical angle to min/max
            m_CameraVerticalAngle = Mathf.Clamp( m_CameraVerticalAngle, -89f, 89f );

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            playerCamera.transform.localEulerAngles = new Vector3( m_CameraVerticalAngle, 0, 0 );
        }

        // character movement handling
        isSprinting = m_InputHandler.GetSprintInputHeld();

        if (isSprinting) {
            isSprinting = SetCrouchingState( false, false );
        }

        float speedModifier = isSprinting ? sprintSpeedModifier : 1f;

        // converts move input to a worldspace vector based on our character's transform orientation
        Vector3 worldspaceMoveInput = transform.TransformVector( m_InputHandler.GetMoveInput() );

        // handle grounded movement
        if (isGrounded) {
            // calculate the desired velocity from inputs, max speed, and current slope
            Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround * speedModifier;
            // reduce speed if crouching by crouch speed ratio
            if (isCrouching)
                targetVelocity *= maxSpeedCrouchedRatio;
            targetVelocity = GetDirectionReorientedOnSlope( targetVelocity.normalized, m_GroundNormal ) * targetVelocity.magnitude;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            characterVelocity = Vector3.Lerp( characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime );

            // jumping
            if (isGrounded && m_InputHandler.GetJumpInputDown()) {
                // force the crouch state to false
                if (SetCrouchingState( false, false )) {
                    // start by canceling out the vertical component of our velocity
                    characterVelocity = new Vector3( characterVelocity.x, 0f, characterVelocity.z );

                    // then, add the jumpSpeed value upwards
                    characterVelocity += Vector3.up * jumpForce;

                    // play sound
                    if (jumpSFX) {
                        audioSource.PlayOneShot( jumpSFX );
                    }

                    // remember last time we jumped because we need to prevent snapping to ground for a short time
                    m_LastTimeJumped = Time.time;
                    hasJumpedThisFrame = true;

                    // Force grounding to false
                    isGrounded = false;
                    m_GroundNormal = Vector3.up;
                }
            }

            // footsteps sound
            float chosenFootstepSFXFrequency = (isSprinting ? footstepSFXFrequencyWhileSprinting : footstepSFXFrequency);
            if (m_footstepDistanceCounter >= 1f / chosenFootstepSFXFrequency) {
                m_footstepDistanceCounter = 0f;
                if (footstepSFX) {
                    audioSource.PlayOneShot( footstepSFX );
                }
            }

            // keep track of distance traveled for footsteps sound
            m_footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;
        }
        // handle air movement
        else {
            // add air acceleration
            characterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;

            // limit air speed to a maximum, but only horizontally
            float verticalVelocity = characterVelocity.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane( characterVelocity, Vector3.up );
            horizontalVelocity = Vector3.ClampMagnitude( horizontalVelocity, maxSpeedInAir * speedModifier );
            characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

            // apply the gravity to the velocity
            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }

        // apply the final calculated velocity value as a character movement
        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere( m_Controller.height );
        m_Controller.Move( characterVelocity * Time.deltaTime );

        // detect obstructions to adjust velocity accordingly
        m_LatestImpactSpeed = Vector3.zero;
        if (Physics.CapsuleCast( capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius, characterVelocity.normalized, out RaycastHit hit, characterVelocity.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore )) {
            // We remember the last impact speed because the fall damage logic might need it
            m_LatestImpactSpeed = characterVelocity;

            characterVelocity = Vector3.ProjectOnPlane( characterVelocity, hit.normal );
        }
    }

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    private bool IsNormalUnderSlopeLimit( Vector3 normal )
    {
        return Vector3.Angle( transform.up, normal ) <= m_Controller.slopeLimit;
    }

    // Gets the center point of the bottom hemisphere of the character controller capsule
    private Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * m_Controller.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule
    private Vector3 GetCapsuleTopHemisphere( float atHeight )
    {
        return transform.position + (transform.up * (atHeight - m_Controller.radius));
    }

    // Gets a reoriented direction that is tangent to a given slope
    public Vector3 GetDirectionReorientedOnSlope( Vector3 direction, Vector3 slopeNormal )
    {
        Vector3 directionRight = Vector3.Cross( direction, transform.up );
        return Vector3.Cross( slopeNormal, directionRight ).normalized;
    }

    private void UpdateCharacterHeight( bool force )
    {
        // Update height instantly
        if (force) {
            m_Controller.height = m_TargetCharacterHeight;
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * cameraHeightRatio;
        }
        // Update smooth height
        else if (m_Controller.height != m_TargetCharacterHeight) {
            // resize the capsule and adjust camera position
            m_Controller.height = Mathf.Lerp( m_Controller.height, m_TargetCharacterHeight, crouchingSharpness * Time.deltaTime );
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.Lerp( playerCamera.transform.localPosition, Vector3.up * m_TargetCharacterHeight * cameraHeightRatio, crouchingSharpness * Time.deltaTime );
        }
    }

    // returns false if there was an obstruction
    private bool SetCrouchingState( bool crouched, bool ignoreObstructions )
    {
        // set appropriate heights
        if (crouched) {
            m_TargetCharacterHeight = capsuleHeightCrouching;
        }
        else {
            // Detect obstructions
            if (!ignoreObstructions) {
                Collider[] standingOverlaps = Physics.OverlapCapsule(
                    GetCapsuleBottomHemisphere(),
                    GetCapsuleTopHemisphere( capsuleHeightStanding ),
                    m_Controller.radius,
                    -1,
                    QueryTriggerInteraction.Ignore );
                foreach (Collider c in standingOverlaps) {
                    if (c != m_Controller) {
                        return false;
                    }
                }
            }

            m_TargetCharacterHeight = capsuleHeightStanding;
        }

        if (onStanceChanged != null) {
            onStanceChanged.Invoke( crouched );
        }

        isCrouching = crouched;
        return true;
    }

    public float GetCameraAngle() => m_CameraVerticalAngle;

    public void SetCameraAngle( float angle )
    {
        m_CameraVerticalAngle = angle;
    }
}