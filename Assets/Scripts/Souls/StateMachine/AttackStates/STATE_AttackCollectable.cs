using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class STATE_AttackCollectable : BaseState
{
    private readonly AttackSoulAgent _owner;

    private InputAction _collectAction;

    private SoulUI _uiController;
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
                    Debug.Log("Soul Collected");

                    if (_uiController != null)
                    {
                        _uiController.SetAttackCollected();
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

        _uiController = GameObject.FindAnyObjectByType<SoulUI>();
    }

    // Runs when we exit this state
    public override void OnExit(BaseState newState)
    {

    }
}