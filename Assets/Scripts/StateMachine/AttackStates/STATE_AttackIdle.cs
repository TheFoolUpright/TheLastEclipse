using System;
using UnityEngine;

public class STATE_AttackIdle : BaseState
{
    private readonly AttackSoulAgent _owner;
    private Transform target;
    private int targetIndex = -1;

    public STATE_AttackIdle(AttackSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick() {
        if (_owner.AgentReachedDestination)
        {
            SetTarget();
        }
        
        return null;
    }
    
    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        Debug.Log("Idle");
        SetTarget();
        _owner.SetStateColor(Color.softRed);
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
        
    }

    private void SetTarget()
    {
        targetIndex = (targetIndex + 1 + _owner.IdlePoints.Count) % _owner.IdlePoints.Count;
        target = _owner.IdlePoints[targetIndex];
        _owner.NavMeshAgent.SetDestination(target.position);
        Debug.Log($"Set Target: {target.name}");
    }
}