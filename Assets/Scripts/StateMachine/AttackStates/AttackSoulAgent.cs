using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AttackSoulAgent : MonoBehaviour
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
     //Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance &&
     agent.velocity.sqrMagnitude == 0f;

    public List<AttackSoulDamageArea> AttackAreas = new List<AttackSoulDamageArea>();
    public List<Transform> WanderingPoints = new List<Transform>();
    public List<Transform> IdlePoints = new List<Transform>();
    [HideInInspector] public PlayerController player;
    
    public int attackCount = 0;
    public bool attackHit;

    public MeshRenderer attackRenderer;

    private void Awake()
    {
        InitializeStateMachine();
        foreach (var item in AttackAreas)
        {
            item.Initialize(this);
        }
    }

    void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            { typeof(STATE_AttackIdle), new STATE_AttackIdle(this) },
            { typeof(STATE_AttackWander), new STATE_AttackWander(this) },
            { typeof(STATE_Attack), new STATE_Attack(this) },
            { typeof(STATE_AttackCollectable), new STATE_AttackCollectable(this) },
        };

        _stateMachine = GetComponent<StateMachine>();
        if (_stateMachine == null)
            _stateMachine = gameObject.AddComponent<StateMachine>();
        _stateMachine.SetStates(states);
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                player = controller;
                if (_stateMachine.CurrentState is STATE_AttackIdle)
                {
                    _stateMachine.SwitchToNewState(typeof(STATE_AttackWander));
                }
            }
        }
    }

    public void SetStateColor(Color color)
    {
        attackRenderer.material.color = color;
    }
}


