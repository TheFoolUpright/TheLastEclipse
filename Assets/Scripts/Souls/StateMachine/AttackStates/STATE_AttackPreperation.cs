using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.AI;

public class STATE_AttackPreperation : BaseState
{
    private readonly AttackSoulAgent _owner;
    private Vector3 targetPosition;
    private int maxSearchAttempts = 30;
    private float searchRadius;
    public STATE_AttackPreperation(AttackSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick() {
        if (_owner.AgentReachedDestination)
        {
            Debug.Log(_owner);
            Debug.Log(_owner.player);
            float playerDistance = Vector3.Distance(targetPosition , _owner.player.transform.position);
            
            if (playerDistance < searchRadius){
                SetTarget();
            } 
            else
            {
                Debug.Log($"Goin to Attack");
                return typeof(STATE_Attack);
            }
        } else
        {
            //Debug.Log("Going to target");
            //Debug.Log("is on navMesh" + _owner.NavMeshAgent.isOnNavMesh);
            //Debug.Log("waiting path" + !_owner.NavMeshAgent.pathPending);
            //Debug.Log("infinite distance" + (_owner.NavMeshAgent.remainingDistance != Mathf.Infinity));
            //Debug.Log("stoping distance" + (_owner.NavMeshAgent.remainingDistance <= _owner.NavMeshAgent.stoppingDistance));
            //Debug.Log("distance to target" + (Vector3.Distance(_owner.transform.position, _owner.NavMeshAgent.destination) <= _owner.NavMeshAgent.stoppingDistance));
            //Debug.Log("velocity" + (_owner.NavMeshAgent.velocity.sqrMagnitude == 0f));
        }
            return null;
    }
    
    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        Debug.Log("Wandering");
        SetTarget();
        _owner.SetStateColor(Color.darkRed);
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
    
    }
    private void SetTarget()
    {
        Vector3 playerPos = _owner.transform.position;
        searchRadius = _owner.AttackAreas[_owner.attackCount].transform.localScale.y * 1.5f;
        for(int i = 0; i < maxSearchAttempts; i++)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 offSet = new Vector3(randomCircle.x, 0f, randomCircle.y) * searchRadius;
            Vector3 candidatePos = playerPos + offSet;

            if(NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
            {
                float distanceToPlayer = Vector3.Distance(hit.position, playerPos);
                if (distanceToPlayer <= searchRadius)
                {
                    targetPosition = hit.position;
                    _owner.NavMeshAgent.SetDestination(targetPosition);
                    Debug.Log($"set target: {targetPosition} ");
                    return;
                }
            }
        }
        Debug.Log("No valid navMesh Point");
        targetPosition = playerPos;
        _owner.NavMeshAgent.SetDestination(targetPosition);
    }
}