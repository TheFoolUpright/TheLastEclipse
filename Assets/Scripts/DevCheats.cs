using UnityEngine;
using UnityEngine.InputSystem;

public class DevCheats : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SoulSceneManager soulSceneManager;
    [SerializeField] private PlayerController playerController;

    private InputAction openPortalAction;
    private InputAction collectSoulAction;
    private InputAction respawnAction;
    private InputAction refillHealthAction;

    private void Awake()
    {
        PlayerInput playerInput = FindAnyObjectByType<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogWarning("No PlayerInput found.");
            return;
        }

        openPortalAction = playerInput.actions["OpenPortalCheat"];
        collectSoulAction = playerInput.actions["CollectSoulCheat"];
        respawnAction = playerInput.actions["RespawnCheat"];
        refillHealthAction = playerInput.actions["RefillHealthCheat"];
    }

    private void Update()
    {
        if (openPortalAction != null && openPortalAction.WasPressedThisFrame())
        {
            OpenPortalCheat();
        }

        if (collectSoulAction != null && collectSoulAction.WasPressedThisFrame())
        {
            CollectMainSoulCheat();
        }

        if (respawnAction != null && respawnAction.WasPressedThisFrame())
        {
            RespawnCheat();
        }

        if (refillHealthAction != null && refillHealthAction.WasPressedThisFrame())
        {
            RefillHealthCheat();
        }
    }

    private void OpenPortalCheat()
    {
        if (soulSceneManager != null)
        {
            soulSceneManager.CollectMainSoul();
            Debug.Log("CHEAT: Portal opened.");
        }
    }

    private void CollectMainSoulCheat()
    {
        if (soulSceneManager != null)
        {
            soulSceneManager.CollectMainSoul();
        }

        SoulUI soulUI = FindAnyObjectByType<SoulUI>();

        if (soulUI != null)
        {
            soulUI.SetFleeCollected();
        }

        Debug.Log("CHEAT: Main soul collected.");
    }

    private void RespawnCheat()
    {
        if (playerController != null)
        {
            playerController.CheatRespawn();
            Debug.Log("CHEAT: Player respawned.");
        }
    }

    private void RefillHealthCheat()
    {
        if (playerController != null)
        {
            playerController.CheatRefillHealth();
            Debug.Log("CHEAT: Health refilled.");
        }
    }
}