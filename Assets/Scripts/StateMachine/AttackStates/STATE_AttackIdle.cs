using System;
using UnityEngine;

public class STATE_AttackIdle : BaseState
{
    private readonly AttackSoulAgent _owner;
    
    public STATE_AttackIdle(AttackSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick() {
        return null;
    }
    
    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        Debug.Log("Idle");
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
    
    }
}