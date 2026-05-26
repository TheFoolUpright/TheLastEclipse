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
    public PlayerController player;
    
    public int attackCount = 0;
    public bool attackHit;
    public float requiredTimeToAttack = 2f;

    public GameObject activeArea;
    public Animator ballAnimator;
    private float startSpeed;

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
            { typeof(STATE_AttackPreperation), new STATE_AttackPreperation(this) },
            { typeof(STATE_Attack), new STATE_Attack(this) },
            { typeof(STATE_AttackCollectable), new STATE_AttackCollectable(this) },
        };

        _stateMachine = GetComponent<StateMachine>();
        if (_stateMachine == null)
            _stateMachine = gameObject.AddComponent<StateMachine>();
        _stateMachine.SetStates(states);
    }

    

    public void OnAttackStateEnter()
    {
        SetStateColor(Color.indianRed);
    }

    public void OnAttackStateExit()
    {
        if (activeArea != null)
        {
            activeArea.SetActive(false);
        }

        if (agent != null)
        {
            agent.speed = startSpeed;
        }
    }

    public void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void PrepareAttackArea()
    {
        activeArea = AttackAreas[attackCount].gameObject;

        // Make sure the attack area is aligned with the enemy
        // before detaching it.
        activeArea.transform.position = transform.position;
        activeArea.transform.rotation = transform.rotation;

        activeArea.SetActive(true);
        activeArea.transform.parent = null;
    }

    public void StartDashAttack()
    {
        activeArea = AttackAreas[attackCount].gameObject;
        activeArea.SetActive(true);

        startSpeed = agent.speed;
        agent.speed *= 10f;

        DashThroughAttackArea();

        ballAnimator.SetBool("Attack", true);
    }

    public void DashThroughAttackArea()
    {
        if (activeArea == null)
            return;

        Vector3 direction = activeArea.transform.forward;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        transform.rotation = Quaternion.LookRotation(direction);

        float dashDistance = GetActiveAreaLength();

        Vector3 dashTarget = activeArea.transform.position + direction * dashDistance;

        agent.SetDestination(dashTarget);
    }

    private float GetActiveAreaLength()
    {
        return AttackAreas[attackCount].GetAttackDistance();
    }

    public void CheckDashFinished()
    {
        if (AgentReachedDestination)
        {
            agent.speed = startSpeed;
        }
    }

    public bool FinishAttack()
    {
        ResetAttackArea();

        ballAnimator.SetBool("Attack", false);
        if (attackHit)
        {
            attackHit = false;
        }
        else
        {
            attackCount++;
        }

        return attackCount >= 3;
    }

    private void ResetAttackArea()
    {
        if (activeArea == null)
            return;

        activeArea.transform.SetParent(transform);
        activeArea.transform.localPosition = Vector3.zero;
        activeArea.transform.localRotation = Quaternion.identity;
    }

    public void SetStateColor(Color color)
    {
        attackRenderer.material.color = color;
    }

}


