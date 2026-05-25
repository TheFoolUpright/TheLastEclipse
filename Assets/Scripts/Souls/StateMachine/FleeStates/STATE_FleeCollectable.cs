using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class STATE_FleeCollectable : BaseState
{
    private readonly FleeSoulAgent _owner;

    private InputAction _collectAction;

    private SoulSceneManager _sceneManager;
    private SoulCollectPopup _popup;

    public STATE_FleeCollectable(FleeSoulAgent owner) : base(owner.gameObject)
    {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick()
    {
        bool playerClose = false;

        Collider[] hits = Physics.OverlapSphere(_owner.transform.position, 1.5f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerClose = true;

                if (_collectAction != null && _collectAction.WasPressedThisFrame())
                {
                    AudioManager.Instance.PlaySFX("FleeCollection");
                    Debug.Log("Soul Collected");

                    if (_popup != null)
                        _popup.Hide();

                    if (_sceneManager != null)
                        _sceneManager.CollectSoul(_owner.gameObject);

                    _owner.gameObject.SetActive(false);
                    return null;
                }
            }
        }

        if (_popup != null)
        {
            if (playerClose)
                _popup.Show();
            else
                _popup.Hide();
        }

        return null;
    }

    // Runs when we enter this state
    public override void OnEnter(BaseState oldState)
    {
        _owner.ClearMovement();
        Debug.Log("Entered Collectable State");

        _owner.ActiveSoul(false);

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

        _sceneManager = GameObject.FindAnyObjectByType<SoulSceneManager>();
        _popup = _owner.GetComponent<SoulCollectPopup>();

        if (_popup != null)
            _popup.Hide();
    }

    // Runs when we exit this state
    public override void OnExit(BaseState newState)
    {
        if (_popup != null)
            _popup.Hide();
    }
}