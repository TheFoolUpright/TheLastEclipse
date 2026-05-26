using System;
using UnityEngine;

public class STATE_Flee : BaseState
{
    private readonly FleeSoulAgent _owner;

    private bool _wasCaught;
    private float _disengageTimer;

    private bool _hasUsedBurstThisFlee;
    private bool _isBursting;
    private float _burstTimer;
    public STATE_Flee(FleeSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs when we enter this state
    public override void OnEnter(BaseState oldState)
    {
        _owner.PlayDetectedPlayerSound();
        _owner.ActiveSoul(true);

        _wasCaught = false;
        _disengageTimer = _owner.FleeDisengageDelay;

        _hasUsedBurstThisFlee = false;
        _isBursting = false;
        _burstTimer = 0f;

        _owner.OnCaughtByPlayer += HandleCaughtByPlayer;
        _owner.SetFleeMovement();
        _owner.SetStateColor(Color.green);

        FleeNode fleePoint = _owner.ChooseFleePoint();
        if (fleePoint != null)
        {
            _owner.MoveTo(fleePoint.transform.position);
        }

        Debug.Log($"Entered Flee State at speed {_owner.GetCurrentBaseFleeSpeed()}");
    }

    // Runs every frame
    public override Type Tick() {
        if (_wasCaught)
        {
            return typeof(STATE_Stunned);
        }

        UpdateDisengageTimer();

        if (_disengageTimer <= 0f)
        {
            return typeof(STATE_FleeWander);
        }

        UpdateBurst();

        if (!_owner.HasActiveFlyingDestination() && _owner.IsTooFarFromFleeGraph())
        {
            FleeNode nearestNode = _owner.ChooseNearestFleeNode();

            if (nearestNode != null)
            {
                _owner.MoveTo(nearestNode.transform.position);
            }

            return null;
        }

        if (_owner.IsCurrentDestinationUnsafe())
        {
            FleeNode saferPoint = _owner.ChooseFleePoint();

            if (saferPoint != null)
            {
                _owner.MoveTo(saferPoint.transform.position);
            }

            return null;
        }

        if (_owner.HasReachedCurrentDestination())
        {
            FleeNode nextFleePoint = _owner.ChooseFleePoint();

            if (nextFleePoint != null)
            {
                _owner.MoveTo(nextFleePoint.transform.position);
            }
        }

        return null;
    }
    
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
        _owner.OnCaughtByPlayer -= HandleCaughtByPlayer;
    }

    private void HandleCaughtByPlayer()
    {
        _wasCaught = true;
        Debug.Log("Soul caught by player");
    }

    private void UpdateDisengageTimer()
    {
        if (_owner.PlayerInDetectionRange)
        {
            _disengageTimer = _owner.FleeDisengageDelay;
            return;
        }

        _disengageTimer -= Time.deltaTime;
    }

    private void UpdateBurst()
    {
        if (!_hasUsedBurstThisFlee && _owner.ShouldTriggerBurst())
        {
            StartBurst();
        }

        if (!_isBursting)
            return;

        _burstTimer -= Time.deltaTime;

        if (_burstTimer <= 0f)
        {
            EndBurst();
        }
    }

    private void StartBurst()
    {
        _hasUsedBurstThisFlee = true;
        _isBursting = true;
        _burstTimer = _owner.BurstDuration;

        _owner.SetBurstMovement();
        _owner.SetStateColor(Color.limeGreen);
        Debug.Log($"Burst started. Speed is now {_owner.GetCurrentBaseFleeSpeed()}");
    }

    private void EndBurst()
    {
        _isBursting = false;
        _burstTimer = 0f;

        _owner.SetFleeMovement();
        _owner.SetStateColor(Color.green);
        Debug.Log($"Burst ended. Speed reset to {_owner.GetCurrentBaseFleeSpeed()}");
    }
}