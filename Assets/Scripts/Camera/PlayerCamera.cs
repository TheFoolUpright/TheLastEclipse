using StarterAssets;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The transform Cinemachine follows. This is usually the same object your CM camera tracks.")]
    public Transform Target;

    [Tooltip("Reference to the player controller so we can read grounded state and vertical speed.")]
    public ThirdPersonController PlayerController;

    [Tooltip("Reference to Starter Assets input script.")]
    public StarterAssetsInputs Input;

#if ENABLE_INPUT_SYSTEM
    [Tooltip("Needed so we can tell whether the player is using mouse or controller.")]
    public PlayerInput PlayerInput;
#endif

    [Header("Manual Camera Rotation")]
    [Tooltip("How fast the camera rotates when using a mouse.")]
    public float MouseSensitivity = 0.05f;

    [Tooltip("How fast the camera rotates when using a controller stick.")]
    public float GamepadSensitivity = 120f;

    [Tooltip("How far up the player can rotate the camera.")]
    public float TopClamp = 70f;

    [Tooltip("How far down the player can rotate the camera.")]
    public float BottomClamp = -30f;

    [Tooltip("Extra pitch added all the time. Useful for small global camera angle tweaks.")]
    public float CameraAngleOverride = 0f;

    [Tooltip("If true, camera rotation input is ignored.")]
    public bool LockCameraPosition = false;

    [Header("Jump / Fall Camera Assist")]
    [Tooltip("Extra downward tilt while rising in the air. Negative numbers look down more.")]
    public float JumpPitchAssist = 8f;

    [Tooltip("Extra downward tilt while falling. Usually stronger than jump assist.")]
    public float FallPitchAssist = 14f;

    [Tooltip("How quickly the airborne tilt blends in and out.")]
    public float AirPitchSmoothSpeed = 8f;

    [Tooltip("How fast the player must be moving upward before jump assist fully starts.")]
    public float UpwardAssistVelocity = 1f;

    [Tooltip("How fast the player must be moving downward before fall assist fully starts.")]
    public float DownwardAssistVelocity = -3f;

    // Current left/right camera angle
    private float _yaw;

    // Current up/down camera angle controlled by the player
    private float _pitch;

    // The extra pitch we add automatically during jumps/falls
    private float _currentAirPitchOffset;

    // Tiny value to ignore microscopic stick / mouse noise
    private const float _threshold = 0.01f;

    private float _airPitchVelocity;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return PlayerInput != null && PlayerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
        }
    }

    private void Start()
    {
        // Fallbacks in case someone forgets to wire references in the Inspector
        if (Input == null)
            Input = GetComponent<StarterAssetsInputs>();

        if (PlayerController == null)
            PlayerController = GetComponent<ThirdPersonController>();

#if ENABLE_INPUT_SYSTEM
        if (PlayerInput == null)
            PlayerInput = GetComponent<PlayerInput>();
#endif

        if (Target != null)
        {
            Vector3 startEuler = Target.rotation.eulerAngles;

            _yaw = startEuler.y;
            _pitch = NormalizePitch(startEuler.x);
        }
    }

    private static float NormalizePitch(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    private void LateUpdate()
    {
        if (Target == null || Input == null || PlayerController == null)
            return;

        HandleManualLookInput();
        ApplyAirbornePitchAssist();
        ApplyFinalRotationToTarget();
    }

    private void HandleManualLookInput()
    {
        // If the camera is locked, do nothing
        if (LockCameraPosition)
            return;

        // Ignore super tiny input so the camera does not drift
        if (Input.look.sqrMagnitude < _threshold)
            return;

        if (IsCurrentDeviceMouse)
        {
            // Mouse input is already frame-based "delta" input,
            // so we usually do NOT multiply it by deltaTime here.
            _yaw += Input.look.x * MouseSensitivity;
            _pitch += Input.look.y * MouseSensitivity;
        }
        else
        {
            // Controller stick input is a continuous value,
            // so multiply by deltaTime to make it frame-rate independent.
            _yaw += Input.look.x * GamepadSensitivity * Time.deltaTime;
            _pitch += Input.look.y * GamepadSensitivity * Time.deltaTime;
        }

        // Clamp the player-controlled pitch so the camera cannot flip over.
        _pitch = ClampAngle(_pitch, BottomClamp, TopClamp);

        // Yaw can spin forever, but we normalize it so the value does not grow forever.
        _yaw = NormalizeAngle(_yaw);
    }

    private void ApplyAirbornePitchAssist()
    {
        // Read information from the player controller
        bool grounded = PlayerController.IsGrounded;
        float verticalVelocity = PlayerController.VerticalVelocity;

        // This is the pitch offset we WANT this frame.
        // Example:
        // 0   = no extra help
        // -10 = tilt down by 10 degrees
        float targetAirPitchOffset = 0f;

        if (!grounded)
        {
            // If moving upward, blend toward the jump assist value.
            if (verticalVelocity > 0f)
            {
                // InverseLerp returns 0 to 1.
                // 0 = barely rising
                // 1 = rising fast enough for full assist
                float jumpT = Mathf.InverseLerp(0f, UpwardAssistVelocity, verticalVelocity);

                targetAirPitchOffset = Mathf.Lerp(0f, JumpPitchAssist, jumpT);
            }
            // If moving downward, blend toward the stronger fall assist value.
            else
            {
                // We invert the order because the velocity is negative while falling.
                // Example:
                // verticalVelocity = 0     -> little/no assist
                // verticalVelocity = -6    -> strong assist
                float fallT = Mathf.InverseLerp(0f, DownwardAssistVelocity, verticalVelocity);

                targetAirPitchOffset = Mathf.Lerp(0f, FallPitchAssist, fallT);
            }
        }

        // Smoothly move toward the desired airborne tilt.
        // This prevents the camera from snapping sharply the moment the player leaves the ground.
        // It eases toward the target over time and reduces snapping.
        float smoothTime = 1f / Mathf.Max(0.01f, AirPitchSmoothSpeed);

        _currentAirPitchOffset = Mathf.SmoothDamp(
            _currentAirPitchOffset,
            targetAirPitchOffset,
            ref _airPitchVelocity,
            smoothTime
        );
    }

    private void ApplyFinalRotationToTarget()
    {
        // Final pitch = player's manual look
        //             + your constant camera tweak
        //             + automatic jump/fall tilt
        float finalPitch = _pitch + CameraAngleOverride + _currentAirPitchOffset;

        // Apply the final rotation to the object Cinemachine follows.
        // Third Person Follow uses this target's orientation to place the camera rig. 
        Target.rotation = Quaternion.Euler(finalPitch, _yaw, 0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);

        // Convert values like 350 into -10 so clamping behaves more intuitively.
        if (angle > 180f)
            angle -= 360f;

        return Mathf.Clamp(angle, min, max);
    }

    private static float NormalizeAngle(float angle)
    {
        while (angle < 0f)
            angle += 360f;

        while (angle >= 360f)
            angle -= 360f;

        return angle;
    }
}

