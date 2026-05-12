using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class STATE_FleeCollectable : BaseState
{
    private readonly FleeSoulAgent _owner;

    private InputAction _collectAction;

    private SoulUI _uiController;
    private SoulSceneManager _sceneManager;

    public STATE_FleeCollectable(FleeSoulAgent owner) : base(owner.gameObject)
    {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick()
    {
        
        Collider[] hits = Physics.OverlapSphere(_owner.transform.position, 1.5f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {

                if (_collectAction != null && _collectAction.WasPressedThisFrame())
                {
                    Debug.Log("Soul Collected");

                    if (_uiController != null)
                    {
                        _uiController.SetFleeCollected();
                    }

                    if (_sceneManager != null)
                    {
                        _sceneManager.CollectMainSoul();
                    }

                    _owner.gameObject.SetActive(false);
                    return null;
                }
            }
        }

        return null;
    }

    // Runs when we enter this state
    public override void OnEnter(BaseState oldState)
    {
        _owner.ClearMovement();
        Debug.Log("Entered Collectable State");

        Rigidbody rb = _owner.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                _collectAction = playerInput.actions["CollectSoul"];
            }
        }

        _owner.SetStateColor(Color.rebeccaPurple);

        _uiController = GameObject.FindAnyObjectByType<SoulUI>();
        _sceneManager = GameObject.FindAnyObjectByType<SoulSceneManager>();
    }

    // Runs when we exit this state
    public override void OnExit(BaseState newState)
    {

    }
}