using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
[RequireComponent(typeof(CharacterController), typeof(AnimationInputHandler), typeof(AudioSource))]
public class AnimationCharacterController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the body used for the player")]
    public Transform playerBody;
    [Tooltip("Audio source for footsteps, jump, etc...")]
    public AudioSource audioSource;

    [Header("General")]
    [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;
    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float groundCheckDistance = 0.05f;

    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float maxSpeedOnGround = 10f;
    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float movementSharpnessOnGround = 15;
    [Tooltip("Max movement speed when crouching")]
    [Range(0, 1)]
    public float maxSpeedCrouchedRatio = 0.5f;
    [Tooltip("Max movement speed when not grounded")]
    public float maxSpeedInAir = 10f;
    [Tooltip("Acceleration speed when in the air")]
    public float accelerationSpeedInAir = 25f;
    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    public float sprintSpeedModifier = 2f;
    [Tooltip("Height at which the player dies instantly when falling off the map")]
    public float killHeight = -50f;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float rotationSpeed = 200f;
    [Range(0.1f, 1f)]
    [Tooltip("Rotation speed multiplier when aiming")]
    public float aimingRotationMultiplier = 0.4f;

    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 9f;
    PhotonView pv;
    [Header("Stance")]
    [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
    public float cameraHeightRatio = 0.9f;
    [Tooltip("Height of character when standing")]
    public float capsuleHeightStanding = 1.8f;
    [Tooltip("Height of character when crouching")]
    public float capsuleHeightCrouching = 0.9f;
    [Tooltip("Speed of crouching transitions")]
    public float crouchingSharpness = 10f;

    [Header("Audio")]
    [Tooltip("Amount of footstep sounds played when moving one meter")]
    public float footstepSFXFrequency = 1f;
    [Tooltip("Amount of footstep sounds played when moving one meter while sprinting")]
    public float footstepSFXFrequencyWhileSprinting = 1f;
    [Tooltip("Sound played for footsteps")]
    public AudioClip footstepSFX;
    [Tooltip("Sound played when jumping")]
    public AudioClip jumpSFX;
    [Tooltip("Sound played when landing")]
    public AudioClip landSFX;
    [Tooltip("Sound played when taking damage froma fall")]
    public AudioClip fallDamageSFX;

    [Header("Fall Damage")]
    [Tooltip("Whether the player will recieve damage when hitting the ground at high speed")]
    public bool recievesFallDamage;
    [Tooltip("Minimun fall speed for recieving fall damage")]
    public float minSpeedForFallDamage = 10f;
    [Tooltip("Fall speed for recieving th emaximum amount of fall damage")]
    public float maxSpeedForFallDamage = 30f;
    [Tooltip("Damage recieved when falling at the mimimum speed")]
    public float fallDamageAtMinSpeed = 10f;
    [Tooltip("Damage recieved when falling at the maximum speed")]
    public float fallDamageAtMaxSpeed = 50f;

    public UnityAction<bool> onStanceChanged;

    public Vector3 characterVelocity { get; set; }
    public bool isGrounded { get; private set; }
    public bool doubleJumpAble { get; private set; }
    public bool hasJumpedThisFrame { get; private set; }
    public bool isDead { get; private set; }
    public bool isCrouching { get; private set; }
    public float RotationMultiplier
    {
        get
        {
            //조준중
            //if (m_WeaponsManager.isAiming)
            //{
            //    return aimingRotationMultiplier;
            //}

            return 1f;
        }
    }

    AnimationInputHandler m_InputHandler;
    CharacterController m_Controller;
    Vector3 m_GroundNormal;
    Vector3 m_CharacterVelocity;
    Vector3 m_LatestImpactSpeed;
    float m_LastTimeJumped = 0f;
    float m_CameraVerticalAngle = 0f;
    float m_footstepDistanceCounter;
    float m_TargetCharacterHeight;

    const float k_JumpGroundingPreventionTime = 0.2f;
    const float k_GroundCheckDistanceInAir = 0.07f;

    AnimationWallRun wallRunComponent;

    [SerializeField] Animator animator;

    void Start()
    {
        // fetch components on the same gameObject
        m_Controller = GetComponent<CharacterController>();

        m_InputHandler = GetComponent<AnimationInputHandler>();

        wallRunComponent = GetComponent<AnimationWallRun>();

        //animator = GetComponent<Animator>();
        /*       audioSource = GetComponent<AudioSource>();*/

        m_Controller.enableOverlapRecovery = true;

        // force the crouch state to false when starting
        SetCrouchingState(false, true);
        UpdateCharacterHeight(true);
        pv = GetComponent<PhotonView>();

    }

    void Update()
    {
        if (pv.IsMine)
        {
            hasJumpedThisFrame = false;

            bool wasGrounded = isGrounded;
            GroundCheck();

            // landing
            if (isGrounded && !wasGrounded)
            {
                // Fall damage
                float fallSpeed = -Mathf.Min(characterVelocity.y, m_LatestImpactSpeed.y);
                float fallSpeedRatio = (fallSpeed - minSpeedForFallDamage) / (maxSpeedForFallDamage - minSpeedForFallDamage);
                if (recievesFallDamage && fallSpeedRatio > 0f)
                {
                    float dmgFromFall = Mathf.Lerp(fallDamageAtMinSpeed, fallDamageAtMaxSpeed, fallSpeedRatio);
                    // fall damage SFX
                    /*  audioSource.PlayOneShot(fallDamageSFX);*/
                }
                else
                {
                    // land SFX
                    /*   audioSource.PlayOneShot(landSFX);*/
                }

                //착지 애니메이션
                animator.SetBool("InAir", false);
            }

            // crouching
            //if (m_InputHandler.GetCrouchInputDown())
            //{
            //   SetCrouchingState(!isCrouching, false);
            //}

            UpdateCharacterHeight(false);

            HandleCharacterMovement();
        }

    }

    void OnDie()
    {
        isDead = true;
    }

    void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = isGrounded ? (m_Controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

        // reset values before the ground check
        isGrounded = false;
        //animator.SetBool("InAir", true);
        m_GroundNormal = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
        {
            // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height), m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore))
            {
                // storing the upward direction for the surface found
                m_GroundNormal = hit.normal;

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                    IsNormalUnderSlopeLimit(m_GroundNormal))
                {
                    isGrounded = true;
                    doubleJumpAble = true;
                    //animator.SetBool("InAir", false);

                    // handle snapping to the ground
                    if (hit.distance > m_Controller.skinWidth)
                    {
                        m_Controller.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    void HandleCharacterMovement()
    {
        // vertical camera rotation
        {
            // limit the camera's vertical angle to min/max
            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            if (wallRunComponent != null)
            {
                //playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
                playerBody.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, wallRunComponent.GetCameraRoll());
            }
            else
            {
                //playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
            }
        }

        // character movement handling
        bool isSprinting = m_InputHandler.GetSprintInputHeld();
        {
            if (isSprinting)
            {
                isSprinting = SetCrouchingState(false, false);
            }

            float speedModifier = isSprinting ? sprintSpeedModifier : 1f;

            // converts move input to a worldspace vector based on our character's transform orientation
            Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

            if (pv.IsMine)
            {
                if (isGrounded || doubleJumpAble || (wallRunComponent != null && wallRunComponent.IsWallRunning()))
                {
                    if (isGrounded)
                    {
                        // calculate the desired velocity from inputs, max speed, and current slope
                        Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround * speedModifier;
                        // reduce speed if crouching by crouch speed ratio
                        if (isCrouching)
                        {
                            targetVelocity *= maxSpeedCrouchedRatio;
                        }
                        targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) * targetVelocity.magnitude;

                        // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                        characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);
                    }
                }
                // handle grounded movement

                // jumping
                if (pv.IsMine)
                {
                    if ((isGrounded || doubleJumpAble || (wallRunComponent != null && wallRunComponent.IsWallRunning())) && m_InputHandler.GetJumpInputDown())
                    {
                        // force the crouch state to false
                        if (SetCrouchingState(false, false))
                        {
                            if (isGrounded)
                            {
                                // start by canceling out the vertical component of our velocity
                                characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
                                // then, add the jumpSpeed value upwards
                                characterVelocity += Vector3.up * jumpForce;
                            }
                            else if (wallRunComponent != null && wallRunComponent.IsWallRunning())
                            {
                                characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
                                // then, add the jumpSpeed value upwards
                                characterVelocity += wallRunComponent.GetWallJumpDirection() * jumpForce * 0.5f;

                                isGrounded = true;
                                doubleJumpAble = true;
                            }
                            else if (doubleJumpAble)
                            {
                                // start by canceling out the vertical component of our velocity
                                characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
                                // then, add the jumpSpeed value upwards
                                characterVelocity += Vector3.up * jumpForce * 1.5f;

                                doubleJumpAble = false;
                            }
                            // play sound
                            /*audioSource.PlayOneShot(jumpSFX);*/

                            // remember last time we jumped because we need to prevent snapping to ground for a short time
                            m_LastTimeJumped = Time.time;
                            hasJumpedThisFrame = true;

                            // Force grounding to false
                            m_GroundNormal = Vector3.up;
                            isGrounded = false;
                        }

                        //점프 애니메이션
                        animator.SetBool("InAir", true);
                    }

                    // footsteps sound
                    float chosenFootstepSFXFrequency = (isSprinting ? footstepSFXFrequencyWhileSprinting : footstepSFXFrequency);
                    if (m_footstepDistanceCounter >= 1f / chosenFootstepSFXFrequency)
                    {
                        m_footstepDistanceCounter = 0f;
                        /* audioSource.PlayOneShot(footstepSFX);*/
                    }

                    // keep track of distance traveled for footsteps sound
                    m_footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;
                }
            }

            if (pv.IsMine)
            {
                if (!isGrounded && (wallRunComponent == null || (wallRunComponent != null && !wallRunComponent.IsWallRunning())))
                {
                    // add air acceleration
                    characterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;

                    // limit air speed to a maximum, but only horizontally
                    float verticalVelocity = characterVelocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
                    horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedModifier);
                    characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                    // apply the gravity to the velocity
                    characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;

                }
            }
        }
        if (pv.IsMine)
        {
            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);
            m_Controller.Move(characterVelocity * Time.deltaTime);

            //이동 애니메이션
            //animator.SetFloat("Velocity", Mathf.Abs(characterVelocity.x + characterVelocity.y) * 40f * Time.deltaTime);
            animator.SetFloat("MoveX", Mathf.Clamp01(Mathf.Abs(characterVelocity.x) * 60f * Time.deltaTime));
            animator.SetFloat("MoveY", Mathf.Clamp01(Mathf.Abs(characterVelocity.z) * 60f * Time.deltaTime));

            // detect obstructions to adjust velocity accordingly
            m_LatestImpactSpeed = Vector3.zero;
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius, characterVelocity.normalized, out RaycastHit hit, characterVelocity.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore))
            {
                // We remember the last impact speed because the fall damage logic might need it
                m_LatestImpactSpeed = characterVelocity;

                characterVelocity = Vector3.ProjectOnPlane(characterVelocity, hit.normal);
                pv.RPC("playerRPC", RpcTarget.All);
            }
        }
        // apply the final calculated velocity value as a character movement

    }

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
    }

    // Gets the center point of the bottom hemisphere of the character controller capsule    
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * m_Controller.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - m_Controller.radius));
    }

    // Gets a reoriented direction that is tangent to a given slope
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }

    void UpdateCharacterHeight(bool force)
    {
        // Update height instantly
        if (force)
        {
            m_Controller.height = m_TargetCharacterHeight;
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            //playerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * cameraHeightRatio;
        }
        // Update smooth height
        else if (m_Controller.height != m_TargetCharacterHeight)
        {
            // resize the capsule and adjust camera position
            m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight, crouchingSharpness * Time.deltaTime);
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            //playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * m_TargetCharacterHeight * cameraHeightRatio, crouchingSharpness * Time.deltaTime);
        }
    }
    [PunRPC]
    public void playerRPC()
    {
        if (transform.position.y < -10f && gameObject.GetComponent<PlayerHealth>().playerHp > 0)
        {
            gameObject.GetComponent<Health>().GetDamage(20f);
        }
    }
    // returns false if there was an obstruction
    bool SetCrouchingState(bool crouched, bool ignoreObstructions)
    {

        // set appropriate heights
        if (crouched)
        {
            m_TargetCharacterHeight = capsuleHeightCrouching;
        }
        else
        {
            // Detect obstructions
            if (!ignoreObstructions)
            {
                Collider[] standingOverlaps = Physics.OverlapCapsule(
                    GetCapsuleBottomHemisphere(),
                    GetCapsuleTopHemisphere(capsuleHeightStanding),
                    m_Controller.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);
                foreach (Collider c in standingOverlaps)
                {
                    if (c != m_Controller)
                    {
                        return false;
                    }
                }
            }

            m_TargetCharacterHeight = capsuleHeightStanding;
        }

        if (onStanceChanged != null)
        {
            onStanceChanged.Invoke(crouched);
        }

        isCrouching = crouched;
        return true;
    }
}