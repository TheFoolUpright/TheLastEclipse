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

    // Current left/right camera angle
    private float _yaw;

    // Current up/down camera angle controlled by the player
    private float _pitch;

    // Tiny value to ignore microscopic stick / mouse noise
    private const float _threshold = 0.01f;

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

    private void LateUpdate()
    {
        if (Target == null || Input == null)
            return;

        HandleManualLookInput();
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

            // Subtract Y so moving the mouse up makes the camera look up
            _pitch -= Input.look.y * MouseSensitivity;
        }
        else
        {
            // Controller stick input is a continuous value,
            // so multiply by deltaTime to make it frame-rate independent.
            _yaw += Input.look.x * GamepadSensitivity * Time.deltaTime;

            // Subtract Y so pushing up makes the camera look up
            _pitch -= Input.look.y * GamepadSensitivity * Time.deltaTime;
        }

        // Clamp the player-controlled pitch so the camera cannot flip over.
        _pitch = ClampAngle(_pitch, BottomClamp, TopClamp);

        // Yaw can spin forever, but we normalize it so the value does not grow forever.
        _yaw = NormalizeAngle(_yaw);
    }

    private void ApplyFinalRotationToTarget()
    {
        // Final pitch = player's manual look + any constant offset
        float finalPitch = _pitch + CameraAngleOverride;

        // Apply the final rotation to the object Cinemachine follows.
        Target.rotation = Quaternion.Euler(finalPitch, _yaw, 0f);
    }

    private static float NormalizePitch(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
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