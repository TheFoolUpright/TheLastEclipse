using System;
using UnityEngine;

public class STATE_Attack : BaseState
{
    private readonly AttackSoulAgent _owner;

    private bool onCooldown;
    private bool toAttack;
    private bool attacking;

    private float timer;

    public STATE_Attack(AttackSoulAgent owner) : base(owner.gameObject)
    {
        _owner = owner;
    }

    public override Type Tick()
    {
        if (onCooldown)
        {
            _owner.FacePlayer();

            timer += Time.deltaTime;

            if (timer >= _owner.requiredTimeToAttack)
            {
                timer = 0f;
                onCooldown = false;
                toAttack = true;

                _owner.PrepareAttackArea();
            }
        }

        if (toAttack)
        {
            timer += Time.deltaTime;

            if (timer >= 2f)
            {
                timer = 0f;
                toAttack = false;
                attacking = true;

                _owner.StartDashAttack();
            }
        }

        if (attacking)
        {
            _owner.CheckDashFinished();

            timer += Time.deltaTime;

            if (timer >= 3f)
            {
                _owner.PlayAttackFeedback();
                timer = 0f;
                attacking = false;

                bool shouldCollect = _owner.FinishAttack();

                if (shouldCollect)
                {
                    return typeof(STATE_AttackCollectable);
                }

                return typeof(STATE_AttackPreperation);
            }
        }

        return null;
    }

    public override void OnEnter(BaseState oldState)
    {
        Debug.Log("Attack");

        onCooldown = true;
        toAttack = false;
        attacking = false;

        timer = 0f;

        _owner.OnAttackStateEnter();
    }

    public override void OnExit(BaseState newState)
    {
        _owner.OnAttackStateExit();
    }
}