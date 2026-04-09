using System;
using UnityEngine;

public class STATE_FleeCollectable : BaseState
{
    private readonly FleeSoulAgent _owner;

    public STATE_FleeCollectable(FleeSoulAgent owner) : base(owner.gameObject)
    {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick()
    {

        // Check of er iets de trigger raakt
        Collider[] hits = Physics.OverlapSphere(_owner.transform.position, 0.5f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Soul Collected!");

                _owner.gameObject.SetActive(false);

                return null;
            }
        }

        return null;
    }

    // Runs when we enter this state
    public override void OnEnter(BaseState oldState)
    {
        _owner.ClearMovement();
        Debug.Log("Entered Collectable State");

        // Zorg dat hij stil staat
        Rigidbody rb = _owner.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    // Runs when we exit this state
    public override void OnExit(BaseState newState)
    {

    }
}