using System;
using UnityEngine;

public class STATE_AttackCollectable : BaseState
{
    private readonly AttackSoulAgent _owner;
    
    public STATE_AttackCollectable(AttackSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick() {
        return null;
    }
    
    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        Debug.Log("Calm");
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
    
    }
}