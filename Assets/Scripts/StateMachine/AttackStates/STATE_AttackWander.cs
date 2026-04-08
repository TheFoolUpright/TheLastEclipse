using NUnit.Framework;
using System;
using UnityEngine;

public class STATE_AttackWander : BaseState
{
    private readonly AttackSoulAgent _owner;
    private float minAttackDistance = 10f;
    private Transform target;
    public STATE_AttackWander(AttackSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick() {
        if (_owner.AgentReachedDestination)
        {
            float playerDistance = Vector3.Distance(target.position , _owner.player.transform.position);
            
            if (playerDistance < minAttackDistance){
                SetTarget();
            } 
            else
            {
                Debug.Log($"Goin to Attack");
                return typeof(STATE_Attack);
            }
        } else
        {
            Debug.Log("Going to target");
            Debug.Log("is on navMesh" + _owner.NavMeshAgent.isOnNavMesh);
            Debug.Log("waiting path" + !_owner.NavMeshAgent.pathPending);
            Debug.Log("infinite distance" + (_owner.NavMeshAgent.remainingDistance != Mathf.Infinity));
            Debug.Log("stoping distance" + (_owner.NavMeshAgent.remainingDistance <= _owner.NavMeshAgent.stoppingDistance));
            Debug.Log("distance to target" + (Vector3.Distance(_owner.transform.position, _owner.NavMeshAgent.destination) <= _owner.NavMeshAgent.stoppingDistance));
            Debug.Log("velocity" + (_owner.NavMeshAgent.velocity.sqrMagnitude == 0f));
        }
            return null;
    }
    
    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        Debug.Log("Wandering");
        SetTarget();
        _owner.SetStateColor(Color.purple);
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
    
    }
    private void SetTarget()
    {
        Transform newTarget = null;

        for(int i = 0 ; i < 1 ; i++)
        {
            newTarget = _owner.WanderingPoints[UnityEngine.Random.Range(0, _owner.WanderingPoints.Count)];
            if(newTarget == target)
            {
                i--;
            }
        }
        target = newTarget;
        _owner.NavMeshAgent.SetDestination(target.position);
        Debug.Log($"Set Target: {target.name}");
    }
}