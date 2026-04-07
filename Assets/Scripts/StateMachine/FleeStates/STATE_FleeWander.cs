using System;
using UnityEngine;

public class STATE_FleeWander : BaseState
{
    private readonly FleeSoulAgent _owner;

    private bool _isWaiting;
    private float _waitTimer;

    public STATE_FleeWander(FleeSoulAgent owner) : base(owner.gameObject)
    {
        _owner = owner;
    }

    // Runs when we enter this state
    public override void OnEnter(BaseState oldState)
    {
        Debug.Log("Entered Wander State");
        _isWaiting = false;
        _waitTimer = 0f;

        _owner.SetWanderMovement();

        Transform wanderPoint = _owner.ChooseRandomWanderPoint();
        if (wanderPoint != null)
        {
            _owner.MoveTo(wanderPoint.position);
        }
    }

    // Runs every frame
    public override Type Tick()
    {
        // If player comes close, stop wandering and start fleeing
        if (_owner.PlayerInDetectionRange)
        {
            return typeof(STATE_Flee);
        }

        // If there are no wander points, do nothing
        if (_owner.WanderingPoints == null || _owner.WanderingPoints.Count == 0)
        {
            return null;
        }

        // After waiting choose the next point
        if (_isWaiting)
        {
            _waitTimer -= Time.deltaTime;

            if (_waitTimer <= 0f)
            {
                _isWaiting = false;

                Transform nextPoint = _owner.ChooseRandomWanderPoint();
                if (nextPoint != null)
                {
                    _owner.MoveTo(nextPoint.position);
                }
            }

            return null;
        }

        // If we've reached the current point, wait a bit
        if (_owner.HasReachedCurrentDestination())
        {
            _owner.StopMoving();

            _isWaiting = true;
            _waitTimer = UnityEngine.Random.Range(_owner.MinWanderIdleTime, _owner.MaxWanderIdleTime);
        }

        return null;
    }


    // Runs when we exit this state
    public override void OnExit(BaseState newState)
    {
        _isWaiting = false;
        _waitTimer = 0f;
    }
}