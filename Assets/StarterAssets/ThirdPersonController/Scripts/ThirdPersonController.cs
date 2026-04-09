using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;


        // This controls how much the jump is shortened when the player
        // lets go of the jump button early.
        //
        // Example:
        // - Holding jump = full jump height
        // - Tapping jump quickly = shorter jump
        //
        // Lower values make the jump get cut more aggressively.
        [Header("Variable Jump Height")]
        [Tooltip("How much upward speed is kept when the jump button is released early. Lower = shorter hop")]
        [Range(0.1f, 1.0f)]
        public float JumpCutMultiplier = 0.5f;


        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        // This tells us whether the player was grounded during the PREVIOUS frame.
        // We use this to detect the exact moment the player lands.
        // Example:
        // - last frame: false
        // - this frame: true
        // => the player just landed
        [Tooltip("Check if player was previously grounded")]
        public bool WasGroundedLastFrame { get; private set; }

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Header("Double Jump")]
        [Tooltip("For restricting how long the player need to wait before double jumping")]
        [SerializeField]
        [Range(0.1f, 1.5f)]
        private float doubleJumpTimer;

        [Header("Coyote Time")]
        [Tooltip("Time (in seconds) you can still jump after leaving ground")]
        public float coyoteMax = .3f;

        [Header("Jump Buffering")]
        [Tooltip("Time (in seconds) you can press jump BEFORE landing and still jump")]
        public float jumpBufferMax = .3f;

        [Header("Camera Sensitivity")]
        [Tooltip("How fast the camera moves with the mouse")]
        public float MouseSensitivity = 0.05f;
        [Tooltip("How fast the camera moves with a controller stick")]
        public float GamepadSensitivity = 120f;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // If the player is standing on a bounce platform,
        // we store a reference to it here.
        // If they are standing on normal ground, this stays null.
        private BouncyPlatform _currentBounceSurface;

        // This is a very short cooldown that stops the bounce from
        // firing over and over across multiple frames.
        // It helps prevent accidental repeated launching.
        [SerializeField] private float bounceRetriggerLock = 0.15f;
        private float _bounceRetriggerTimer = 0f;
        // True only when the current upward launch came from a player jump
        // that should support variable jump height.
        // Bounce launches should usually set this to false.
        private bool _allowVariableJumpCut = false;

        // double jump
        private bool canDoubleJump = true;
        private float _doubleJumpTimer = 0.1f;

        // coyote time
        private float coyoteCounter = 0;

        // jump buffering
        private float jumpBufferCounter = 0;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;


#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            _doubleJumpTimer = doubleJumpTimer;
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            // Before we do a new grounded check,
            // remember whether we were grounded last frame.
            // This lets us compare old state vs new state.
            WasGroundedLastFrame = Grounded;

            // Reduce the bounce cooldown timer over time.
            // Once this reaches 0, bouncing is allowed again.
            if (_bounceRetriggerTimer > 0f)
                _bounceRetriggerTimer -= Time.deltaTime;

            GroundedCheck();

            // After checking what ground is under the player,
            // see whether we just landed on a bounce platform.
            HandleBounceLanding();

            JumpAndGravity();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - GroundedOffset,
                transform.position.z
            );

            // Instead of only asking "am I touching ground?",
            // we gather all colliders in the ground check sphere.
            // That way we can inspect the surface we landed on.
            Collider[] hits = Physics.OverlapSphere(
                spherePosition,
                GroundedRadius,
                GroundLayers,
                QueryTriggerInteraction.Ignore
            );

            // If we touched at least one valid ground collider,
            // the player is considered grounded.
            Grounded = hits.Length > 0;

            // Reset this every frame before checking the colliders.
            // If we find a bounce platform below us, we store it here.
            _currentBounceSurface = null;

            foreach (Collider hit in hits)
            {
                // Check whether this ground collider belongs to a bounce platform.
                // If yes, save a reference so we can use its bounce settings.
                BouncyPlatform bounceSurface = hit.GetComponent<BouncyPlatform>();
                if (bounceSurface != null)
                {
                    _currentBounceSurface = bounceSurface;
                    break;
                }
            }

            if (Grounded)
                canDoubleJump = true;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // Only rotate the camera if:
            // 1. The player is actually moving the camera (input is not tiny)
            // 2. The camera is not locked
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                // Check if we are using mouse or controller
                if (IsCurrentDeviceMouse)
                {
                    // MOUSE INPUT

                    // Add horizontal input (left/right) to yaw (turning left/right)
                    // Multiply by sensitivity to control speed
                    _cinemachineTargetYaw += _input.look.x * MouseSensitivity;

                    // Add vertical input (up/down) to pitch (looking up/down)
                    _cinemachineTargetPitch += _input.look.y * MouseSensitivity;
                }
                else
                {
                    // CONTROLLER INPUT

                    // Controller input is NOT frame-based, so we multiply by deltaTime
                    // This keeps movement smooth and consistent across frame rates

                    _cinemachineTargetYaw += _input.look.x * GamepadSensitivity * Time.deltaTime;
                    _cinemachineTargetPitch += _input.look.y * GamepadSensitivity * Time.deltaTime;
                }
            }

            // Clamp = limit values so they don't go crazy

            // Yaw (left/right) can spin forever, so we just normalize it
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);

            // Pitch (up/down) is clamped so you can't flip upside down
            // BottomClamp = how far you can look down
            // TopClamp = how far you can look up
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Apply rotation to the camera target
            // Cinemachine will follow this object

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
                _cinemachineTargetPitch + CameraAngleOverride, // up/down
                _cinemachineTargetYaw,                         // left/right
                0.0f                                           // no roll (tilting sideways)
            );
        }


        private void Move()
        {
            // Choose movement speed.
            // By default, the player runs.
            // If walk mode has been toggled on, use walk speed instead.
            float targetSpeed = _input.walk ? MoveSpeed : SprintSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            //Debug.Log("currentHorizontalSpeed: "+currentHorizontalSpeed);

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            // ========================
            // COYOTE TIME (grace period after leaving ground)
            // ========================
            // If grounded, reset the coyote timer
            if (Grounded)
                coyoteCounter = coyoteMax;
            // If in air, count it down
            else
                coyoteCounter -= Time.deltaTime;

            // ========================
            // JUMP BUFFERING 
            // ========================
            // If the player pressed jump THIS FRAME, start the buffer timer
            if (_input.jumpPressedThisFrame)
                jumpBufferCounter = jumpBufferMax;
            // Otherwise, count the buffer down over time
            else
                jumpBufferCounter -= Time.deltaTime;

            // ========================
            // DOUBLE JUMP TIMER
            // ========================
            // This creates a small delay before the player is allowed to double jump
            if (!Grounded && canDoubleJump && _doubleJumpTimer > 0)
                _doubleJumpTimer -= Time.deltaTime;

            // ========================
            // JUMP COOLDOWN TIMER
            // ========================
            // Prevents spamming the jump button
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            // ========================
            // JUMP INPUT HANDLING
            // ========================
            // Allow jump if:
            // - player pressed jump recently (buffer > 0)
            // - jump cooldown is finished
            if (jumpBufferCounter > 0 && _jumpTimeoutDelta <= 0.0f)
            {
                // ------------------------
                // NORMAL JUMP (ground OR coyote time)
                // ------------------------
                if (Grounded || coyoteCounter > 0)
                {

                    // Calculate jump velocity based on desired jump height
                    // (this uses physics formula)
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // Allow for varible jump height
                    _allowVariableJumpCut = true;

                    // Reset jump cooldown
                    _jumpTimeoutDelta = JumpTimeout;

                    // Clear the jump buffer so it doesn't trigger another jump
                    jumpBufferCounter = 0;

                    // negate coyote time so it can't be reused
                    coyoteCounter = 0;

                    // Trigger jump animation
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }

                    // Reset double jump delay timer
                    _doubleJumpTimer = doubleJumpTimer;


                }
                // ------------------------
                // DOUBLE JUMP (mid-air)
                // ------------------------
                else if (!Grounded && canDoubleJump && _doubleJumpTimer <= 0f)
                {

                    // Slightly weaker jump than the first jump
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight / 1.5f * -2f * Gravity);

                    // Allow for varible jump height
                    _allowVariableJumpCut = true;

                    // Disable further double jumps until grounded again
                    canDoubleJump = false;

                    // negate the jump buffer
                    jumpBufferCounter = 0;

                }
            }

            // ========================
            // GROUNDED BEHAVIOR
            // ========================
            if (Grounded)
            {
                // Don't allow varible jump height
                _allowVariableJumpCut = false;

                // reset the fall timeout timer (used for animations)
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                // Reset jump/fall animations
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                // Prevent player from building up downward speed when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f; // small downward force keeps player grounded
                }
            }

            // ========================
            // AIRBORNE BEHAVIOR
            // ========================
            else
            {

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // Trigger falling animation after delay
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

            }

            // ========================
            // VARIABLE JUMP HEIGHT
            // ========================
            // If the player releases jump while still moving upward,
            // cut the upward velocity short to create a smaller hop.
            if (_allowVariableJumpCut && !Grounded && _verticalVelocity > 0.0f && !_input.jump)
            {
                _verticalVelocity *= JumpCutMultiplier;
                _allowVariableJumpCut = false;
            }

            // ========================
            // GRAVITY
            // ========================
            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void HandleBounceLanding()
        {
            // If the short bounce cooldown is still active,
            // do nothing yet.
            if (_bounceRetriggerTimer > 0f)
                return;

            // The player "just landed" if:
            // - they are grounded now
            // - but they were NOT grounded last frame
            bool justLanded = Grounded && !WasGroundedLastFrame;

            if (!justLanded)
                return;

            // If there is no bounce platform under the player,
            // then this is just a normal landing.
            if (_currentBounceSurface == null)
                return;

            // Apply the bounce using the settings from the platform.
            Bounce(_currentBounceSurface.BounceHeight, _currentBounceSurface.ResetDoubleJump);

            // Tell the platform to play its own feedback
            // such as audio, particles, or squash animation.
            _currentBounceSurface.PlayBounceFeedback();

            // Start the short lock so the bounce doesn't trigger again instantly.
            _bounceRetriggerTimer = bounceRetriggerLock;
        }

        public void Bounce(float bounceHeight, bool resetDoubleJump = true, bool clearJumpBuffer = true)
        {
            // Convert the desired bounce height into upward velocity.
            // This uses the same jump physics idea as the normal jump.
            _verticalVelocity = Mathf.Sqrt(bounceHeight * -2f * Gravity);
            
            // Don't allow varible jump height
            _allowVariableJumpCut = false;

            // Optional: clear jump buffering so a stored jump press
            // does not accidentally trigger immediately after bouncing.
            if (clearJumpBuffer)
                jumpBufferCounter = 0f;

            // Clear coyote time because the bounce is a fresh launch,
            // not a leftover "just left the ground" state.
            coyoteCounter = 0f;

            // Optionally restore double jump after bouncing.
            // This is useful if you want the bounce platform to feel generous.
            if (resetDoubleJump)
            {
                canDoubleJump = true;
                _doubleJumpTimer = 0f;
            }

            // Trigger jump-style animation states.
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, true);
                _animator.SetBool(_animIDFreeFall, false);
            }

         
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}