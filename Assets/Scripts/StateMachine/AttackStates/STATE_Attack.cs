using System;
using UnityEngine;

public class STATE_Attack : BaseState
{
    private readonly AttackSoulAgent _owner;
    private bool onCooldown;
    private bool attacking;
    private float timer;
    private GameObject activeArea;
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
            if (timer >= 2)
            {
                timer = 0;
                onCooldown = false;
                activeArea = _owner.AttackAreas[_owner.attackCount].gameObject;
                activeArea.SetActive(true);
                attacking = true;
            }
        }
        if (attacking)
        {
            timer += Time.deltaTime;
            if(timer >= 3)
            {
                if (_owner.attackHit)
                {
                    _owner.attackHit = false;

                } else
                {
                    _owner.attackCount++;
                }

                if (_owner.attackCount >= 3)
                {
                    return typeof(STATE_AttackCollectable);
                }
                return typeof(STATE_AttackWander);
            }
        }

        return null;
     
    }

    // Runs when we enter this state
    public override void OnEnter(BaseState oldState){
        Debug.Log("Attack");
        onCooldown = true;
        timer = 0;
        _owner.SetStateColor(Color.rebeccaPurple);
    }
    
    // Runs when we exit this state
    public override void OnExit(BaseState newState) {
        activeArea.SetActive(false);
    }
}