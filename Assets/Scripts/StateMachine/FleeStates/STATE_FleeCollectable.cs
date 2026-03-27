using System;
using UnityEngine;

public class STATE_FleeCollectable : BaseState
{
    private readonly FleeSoulAgent _owner;
    
    public STATE_FleeCollectable(FleeSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick() {
        return null;
    }
    
    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
    
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
    
    }
}