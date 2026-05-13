using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class STATE_AttackCollectable : BaseState
{
    private readonly AttackSoulAgent _owner;

    private InputAction _collectAction;

    private SoulSceneManager _sceneManager;
    public STATE_AttackCollectable(AttackSoulAgent owner) : base(owner.gameObject)
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
                    Debug.Log("Attack Soul Collected");

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
        Debug.Log("Entered Collectable State");


        if (_owner.NavMeshAgent != null && _owner.NavMeshAgent.isOnNavMesh)
        {
            _owner.NavMeshAgent.isStopped = true;
            _owner.NavMeshAgent.ResetPath();
            _owner.SetStateColor(Color.rebeccaPurple);
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

        _sceneManager = GameObject.FindAnyObjectByType<SoulSceneManager>();
    }

    // Runs when we exit this state
    public override void OnExit(BaseState newState)
    {

    }
}