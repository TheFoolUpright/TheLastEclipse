using System;
using UnityEngine;

public class STATE_Attack : BaseState
{
    private readonly AttackSoulAgent _owner;
    private bool onCooldown;
    private float cooldownDuration;
    private float timer;
    public STATE_Attack(AttackSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick()
    {
        if(_owner.attackCount >= 3)
        {
            return typeof(STATE_AttackCollectable);
        }
            return typeof(STATE_AttackWander);
    }

    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        Debug.Log("Attack");
        _owner.attackCount++;
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
    
    }
}