using UnityEngine;
using UnityEngine.InputSystem;

public class DevCheats : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SoulSceneManager soulSceneManager;
    [SerializeField] private HubProgressManager hubProgressManager;
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

    private void Start()
    {
        if (soulSceneManager == null)
            soulSceneManager = FindAnyObjectByType<SoulSceneManager>();

        if (hubProgressManager == null)
            hubProgressManager = FindAnyObjectByType<HubProgressManager>();

        if (playerController == null)
            playerController = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (openPortalAction != null && openPortalAction.WasPressedThisFrame())
            OpenPortalCheat();

        if (collectSoulAction != null && collectSoulAction.WasPressedThisFrame())
            CollectMainSoulCheat();

        if (respawnAction != null && respawnAction.WasPressedThisFrame())
            RespawnCheat();

        if (refillHealthAction != null && refillHealthAction.WasPressedThisFrame())
            RefillHealthCheat();
    }

    private void OpenPortalCheat()
    {
        if (soulSceneManager != null)
        {
            soulSceneManager.CollectMainSoul();
            Debug.Log("CHEAT: Arena return portal opened.");
        }
        else if (hubProgressManager != null)
        {
            hubProgressManager.CheatCollectAllMainSouls();
            Debug.Log("CHEAT: Hub final portal opened.");
        }
        else
        {
            Debug.LogWarning("CHEAT FAILED: No SoulSceneManager or HubProgressManager found.");
        }
    }

    private void CollectMainSoulCheat()
    {
        if (soulSceneManager != null)
        {
            soulSceneManager.CollectMainSoul();
            Debug.Log("CHEAT: Current arena main soul collected.");
        }
        else if (hubProgressManager != null)
        {
            hubProgressManager.CheatCollectAllMainSouls();
        }
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