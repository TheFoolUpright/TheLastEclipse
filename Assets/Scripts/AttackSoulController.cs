using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AttackSoulController : MonoBehaviour
{
    private AttackSoulState currentState;
    private int attackCount = 0;
    private void SetState(AttackSoulState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case AttackSoulState.Attacking:
                StartCoroutine(AttackRoutine());
                break;
            case AttackSoulState.Cooldown:
                StartCoroutine(CooldownRoutine());
                break;
            case AttackSoulState.Calm:
                Calm();
                break;
        }
    }

    IEnumerator CooldownRoutine()
    {
        Debug.Log("Cooldown");
        yield return new WaitForSeconds(2);
        SetState(AttackSoulState.Attacking);
    }

    private IEnumerator AttackRoutine()
    {
        Debug.Log("Attacking");
        yield return new WaitForSeconds(2);
        attackCount++;
        if (attackCount < 3) SetState(AttackSoulState.Cooldown);
        else SetState(AttackSoulState.Calm);
       
    }
    private void Start()
    {
        SetState(AttackSoulState.Wander);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState != AttackSoulState.Wander) return;

        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                SetState(AttackSoulState.Attacking);
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        switch (currentState)
        {
            case AttackSoulState.Wander:
                Wander();
                break;
            case AttackSoulState.Calm:
                Calm();
                break;
        }
    }

    private void Calm()
    {
        Debug.Log("Calm");
    }
    private void Wander()
    {
        Debug.Log("Wander");
    }
}

public enum AttackSoulState
{
    Wander,
    Attacking,
    Cooldown,
    Calm
}
