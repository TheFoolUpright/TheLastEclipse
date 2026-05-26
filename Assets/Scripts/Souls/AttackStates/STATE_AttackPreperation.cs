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
        //if (targetPosition == Vector3.zero)
        //{
            //SetTarget();
            //return null;
        //}
        if (_owner.AgentReachedDestination)
        {
            Debug.Log(_owner);
            Debug.Log(_owner.player);
            float playerDistance = Vector3.Distance(_owner.transform.position , _owner.player.transform.position);
            
            if (playerDistance < searchRadius * 0.5f || playerDistance > searchRadius){
                SetTarget();
            } 
            else
            {
                Debug.Log($"Goin to Attack");
                return typeof(STATE_Attack);
            }
        }
            return null;
    }
    
    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        Debug.Log("Wandering");
        //SetTarget();
        searchRadius = _owner.AttackAreas[_owner.attackCount].GetAttackDistance() * 0.75f;

        if (_owner.attackCount > 0)
        {
            SetTarget();
        }
        _owner.SetStateColor(Color.darkRed);
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
    
    }
    private void SetTarget(int round = 0)
    {
        Vector3 playerPos = _owner.player.transform.position;
        searchRadius = _owner.AttackAreas[_owner.attackCount].GetAttackDistance() * 0.75f;
        for(int i = 0; i < maxSearchAttempts; i++)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 offSet = new Vector3(randomCircle.x, 0f, randomCircle.y) * searchRadius;
            Vector3 candidatePos = playerPos + offSet * (0.75f + 0.25f * round);

            if(NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                float distanceToPlayer = Vector3.Distance(hit.position, playerPos);
                if (distanceToPlayer >= searchRadius)
                {
                    targetPosition = hit.position;
                    _owner.NavMeshAgent.SetDestination(targetPosition);
                    Debug.Log($"set target: {targetPosition} ");
                    return;
                }
            }
        }
        if(round < 5)
        {
            SetTarget(round + 1);
        } else
        {
            Debug.Log("No valid navMesh Point");
            targetPosition = playerPos;
            _owner.NavMeshAgent.SetDestination(targetPosition);
        }

    }
}