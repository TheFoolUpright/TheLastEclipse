using System;
using UnityEngine;

public class STATE_AttackWander : BaseState
{
    private readonly AttackSoulAgent _owner;
    private float minAttackDistance = 10f;
    
    public STATE_AttackWander(AttackSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick() {
        return typeof(STATE_Attack);
    }
    
    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        Debug.Log("Wandering");
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
    
    }
}