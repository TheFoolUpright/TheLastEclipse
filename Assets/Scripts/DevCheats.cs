using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DevCheats : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SoulSceneManager soulSceneManager;
    [SerializeField] private HubProgressManager hubProgressManager;
    [SerializeField] private PlayerController playerController;

    [Header("Scene Cheats")]
    [SerializeField] private string attackArenaSceneName;
    [SerializeField] private string fleeArenaSceneName;

    [SerializeField] private string attackArenaSpawnPointName;
    [SerializeField] private string fleeArenaSpawnPointName;

    private InputAction openPortalAction;
    private InputAction collectSoulAction;
    private InputAction respawnAction;
    private InputAction refillHealthAction;
    private InputAction goToAttackArenaAction;
    private InputAction goToFleeArenaAction;

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
        goToAttackArenaAction = playerInput.actions["GoToAttackArenaCheat"];
        goToFleeArenaAction = playerInput.actions["GoToFleeArenaCheat"];
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

        if (goToAttackArenaAction != null && goToAttackArenaAction.WasPressedThisFrame())
        {
            GoToAttackArenaCheat();
        }

        if (goToFleeArenaAction != null && goToFleeArenaAction.WasPressedThisFrame())
        {
            GoToFleeArenaCheat();
        }
    }

    private void GoToAttackArenaCheat()
    {
        PortalSpawnData.spawnPointName = attackArenaSpawnPointName;
        SceneManager.LoadScene(attackArenaSceneName);

        Debug.Log("CHEAT: Loading Attack Arena");
    }

    private void GoToFleeArenaCheat()
    {
        PortalSpawnData.spawnPointName = fleeArenaSpawnPointName;
        SceneManager.LoadScene(fleeArenaSceneName);

        Debug.Log("CHEAT: Loading Flee Arena");
    }

    private void OpenPortalCheat()
    {
        if (soulSceneManager != null)
        {
            soulSceneManager.CheatCollectSceneMainSoul();
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
            soulSceneManager.CheatCollectSceneMainSoul();
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