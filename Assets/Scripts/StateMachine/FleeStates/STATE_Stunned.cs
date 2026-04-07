using System;
using UnityEngine;

public class STATE_Stunned : BaseState
{
    private readonly FleeSoulAgent _owner;

    private float _stunnedTimer;
    private bool _catchCountApplied;
    public STATE_Stunned(FleeSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs when we enter this state
    public override void OnEnter(BaseState oldState)
    {
        _stunnedTimer = _owner.StunnedDuration;
        _catchCountApplied = false;

        _owner.ClearMovement();

        ApplyCatchCount();
        Debug.Log($"Entered Stunned State. Catch Count: {_owner.CurrentCatchCount}");
    }

    // Runs every frame
    public override Type Tick() {
        if (_owner.HasMetCatchRequirement())
        {
            return typeof(STATE_FleeCollectable);
        }

        _stunnedTimer -= Time.deltaTime;

        if (_stunnedTimer <= 0f)
        {
            return typeof(STATE_Flee);
        }

        return null;
    }
    
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
    
    }

    private void ApplyCatchCount()
    {
        if (_catchCountApplied)
            return;

        _owner.IncrementCatchCount();
        _catchCountApplied = true;
    }
}