using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeSoulAgent : MonoBehaviour
{
    private StateMachine _stateMachine;
    public StateMachine StateMachine => _stateMachine;


    [SerializeField] private NavMeshAgent agent;
    public NavMeshAgent NavMeshAgent => agent;

    public bool AgentReachedDestination =>
        agent.isOnNavMesh &&
        !agent.pathPending &&
        agent.remainingDistance != Mathf.Infinity &&
        agent.remainingDistance <= agent.stoppingDistance &&
        Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance &&
        agent.velocity.sqrMagnitude == 0f;

    [SerializeField] private GameObject chaseObject;

    public event Action<Collider> OnTriggerExitEvent;
    public event Action<Collider> OnTriggerEnterEvent;
    public List<Vector3> WanderingPoints = new List<Vector3>();


    private void Awake()
    {
        InitializeStateMachine();
    }

    void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            { typeof(STATE_Wander), new STATE_Wander(this) },
            { typeof(STATE_Flee), new STATE_Flee(this) },
            { typeof(STATE_Collectable), new STATE_Collectable(this) },
        };

        _stateMachine = GetComponent<StateMachine>();
        if (_stateMachine == null)
            _stateMachine = gameObject.AddComponent<StateMachine>();
        _stateMachine.SetStates(states);
    }

    private void Update()
    {
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var point in WanderingPoints)
        {
            Gizmos.DrawSphere(point, 0.5f);
        }
    }
}

public enum AIState
{
    WANDER,
    FLEE,
    COLLECTABLE
}