using System;
using UnityEngine;

public abstract class BaseState
{
    protected GameObject GameObject;
    protected Transform Transform;

    public BaseState(GameObject gameObject)
    {
        this.GameObject = gameObject;
        this.Transform = gameObject.transform;
    }

    public abstract Type Tick();
    public abstract void OnEnter(BaseState oldState);
    public abstract void OnExit(BaseState newState);
}
