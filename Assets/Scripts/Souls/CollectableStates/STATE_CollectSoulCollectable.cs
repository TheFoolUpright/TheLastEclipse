

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class STATE_CollectSoulCollectable : BaseState
{
    private readonly CollectableSoulAgent _owner;

    private InputAction _collectAction;
    private SoulSceneManager _sceneManager;
    private SoulCollectPopup _popup;

    public STATE_CollectSoulCollectable(CollectableSoulAgent owner) : base(owner.gameObject)
    {
        _owner = owner;
    }

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
                    Debug.Log("Hidden Soul Collected");

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

    public override void OnEnter(BaseState oldState)
    {
        Debug.Log("Entered Hidden Soul Collectable State");

        _owner.SetStateColor(Color.rebeccaPurple);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();

            if (playerInput != null)
                _collectAction = playerInput.actions["CollectSoul"];
        }

        _sceneManager = GameObject.FindAnyObjectByType<SoulSceneManager>();

        _popup = _owner.GetComponent<SoulCollectPopup>();

        if (_popup != null)
            _popup.Hide();
    }

    public override void OnExit(BaseState newState)
    {
        if (_popup != null)
            _popup.Hide();
    }
}