using System;
using UnityEngine;

public class STATE_Attack : BaseState
{
    private readonly AttackSoulAgent _owner;
    private bool onCooldown;
    private float cooldownDuration;
    private float timer;
    private float attackDistance = 3f;
    public STATE_Attack(AttackSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick()
    {
        if (_owner.AgentReachedDestination)
        {
            _owner.NavMeshAgent.speed /= 10;
            if(Vector3.Distance(_owner.transform.position, _owner.player.transform.position) <= attackDistance)
            {
                _owner.player.Damage();
            } else
            {
                _owner.attackCount++;
            }
            if (_owner.attackCount >= 3)
            {
                return typeof(STATE_AttackCollectable);
            }
            return typeof(STATE_AttackWander);
        }
        return null;
    }

    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        _owner.NavMeshAgent.speed *= 10;
        _owner.NavMeshAgent.SetDestination(_owner.player.transform.position);
        Debug.Log("Attack");
 
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
    
    }
}