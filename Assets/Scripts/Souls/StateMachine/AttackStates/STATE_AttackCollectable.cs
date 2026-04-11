using System;
using UnityEngine;

public class STATE_AttackCollectable : BaseState
{
    private readonly AttackSoulAgent _owner;

    public STATE_AttackCollectable(AttackSoulAgent owner) : base(owner.gameObject)
    {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick()
    {
        // Check of er iets de trigger raakt
        Collider[] hits = Physics.OverlapSphere(_owner.transform.position, 0.5f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Soul Collected!");

                _owner.gameObject.SetActive(false);

                return null;
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
        }

    }

    // Runs when we exit this state
    public override void OnExit(BaseState newState)
    {

    }
}