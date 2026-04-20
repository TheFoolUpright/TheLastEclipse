using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public Dictionary<Type, BaseState> AvailableStates { get; private set; }
    public BaseState CurrentState { get; private set; }
    public event Action<BaseState, BaseState> OnStateChanged;

    public StateSharedData SharedData;

    [SerializeField] private string CurrentStateName;

    public void SetStates(Dictionary<Type, BaseState> states)
    {
        AvailableStates = states;
    }

    private void Update()
    {
        if (CurrentState == null)
            SwitchToNewState(AvailableStates.Values.First().GetType());

        var nextState = CurrentState?.Tick();

        if (nextState != null &&
            nextState != CurrentState?.GetType())
            SwitchToNewState(nextState);
    }

    public void SwitchToNewState(Type nextState)
    {
        Debug.Log("Swapping to state " + nextState.ToString());
        BaseState oldState = CurrentState;
        BaseState newState = AvailableStates[nextState];
        CurrentState?.OnExit(newState);
        CurrentState = newState;
        CurrentState.OnEnter(oldState);
        OnStateChanged?.Invoke(oldState, newState);
        CurrentStateName = CurrentState.GetType().Name;
    }
}

public class StateSharedData
{
    public string Key { get; private set; }
    public string Value { get; private set; }

    public StateSharedData(string key, string value)
    {
        this.Key = key;
        this.Value = value;
    }
}