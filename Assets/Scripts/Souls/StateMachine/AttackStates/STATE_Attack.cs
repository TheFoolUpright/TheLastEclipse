using System;
using UnityEngine;

public class STATE_Attack : BaseState
{
    private readonly AttackSoulAgent _owner;
    private bool onCooldown;
    private bool attacking;
    private bool toAttack;
    private float timer;
    private float startSpeed;
    public GameObject activeArea;
    public STATE_Attack(AttackSoulAgent owner) : base(owner.gameObject) {
        _owner = owner;
    }

    // Runs every frame
    public override Type Tick()
    {
        if (onCooldown)
        {
            Vector3 direction = _owner.player.transform.position - _owner.transform.position;
            _owner.transform.rotation = Quaternion.LookRotation(direction);
            timer += Time.deltaTime;
            
            //if (_owner.attackCount >= 1)
            //{
            //    _owner.requiredTimeToAttack += 1f;
            //}

            if (timer >= _owner.requiredTimeToAttack)
            {
                timer = 0;
                onCooldown = false;
                activeArea = _owner.AttackAreas[_owner.attackCount].gameObject;
                activeArea.SetActive(true);
                toAttack = true;
                activeArea.transform.parent = null;
            }
        }
        if (toAttack)
        {
            timer += Time.deltaTime;
            if (timer >= 2)
            {
                timer = 0;
                toAttack = false;
                activeArea = _owner.AttackAreas[_owner.attackCount].gameObject;
                activeArea.SetActive(true);
                attacking = true;
                startSpeed = _owner.NavMeshAgent.speed;
                _owner.NavMeshAgent.speed *= 10;
                _owner.NavMeshAgent.SetDestination(_owner.AttackAreas[_owner.attackCount].endTarget.position);
            }
        }
        if (attacking)
        {
            if (_owner.AgentReachedDestination)
            {
                _owner.NavMeshAgent.speed = startSpeed;
            }
            timer += Time.deltaTime;
            if(timer >= 3)
            {
                activeArea.transform.parent = _owner.transform;
                activeArea.transform.localPosition = Vector3.zero;
                activeArea.transform.localRotation = Quaternion.identity;

                if (_owner.attackHit)
                {
                    _owner.attackHit = false;
                } 
                else
                {
                    _owner.attackCount++;
                }

                if (_owner.attackCount >= 3)
                {
                    return typeof(STATE_AttackCollectable);
                }
                return typeof(STATE_AttackPreperation);
            }
        }

        return null;
     
    }

    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        Debug.Log("Attack");
        onCooldown = true;
        timer = 0;
        _owner.SetStateColor(Color.indianRed);
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
        activeArea?.SetActive(false);
    }
}