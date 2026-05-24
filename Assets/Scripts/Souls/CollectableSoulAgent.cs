using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSoulAgent : MonoBehaviour
{
    private StateMachine _stateMachine;
    public StateMachine StateMachine => _stateMachine;

    [Header("Visuals")]
    [SerializeField] private MeshRenderer soulRenderer;

    private void Awake()
    {
        InitializeStateMachine();
    }

    private void Start()
    {
        _stateMachine.SwitchToNewState(typeof(STATE_CollectSoulCollectable));
    }

    private void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            { typeof(STATE_CollectSoulCollectable), new STATE_CollectSoulCollectable(this) },
        };

        _stateMachine = GetComponent<StateMachine>();

        if (_stateMachine == null)
            _stateMachine = gameObject.AddComponent<StateMachine>();

        _stateMachine.SetStates(states);
    }

    public void SetStateColor(Color color)
    {
        if (soulRenderer != null)
            soulRenderer.material.color = color;
    }
}