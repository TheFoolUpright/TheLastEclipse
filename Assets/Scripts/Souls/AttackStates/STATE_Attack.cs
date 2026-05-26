using System;
using UnityEngine;

public class STATE_Attack : BaseState
{
    private readonly AttackSoulAgent _owner;

    private string attackSound = "SoulAttack";
    public void PlayAttackFeedback()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(attackSound);
        }
    }

    public STATE_Attack(AttackSoulAgent owner) : base(owner.gameObject)
    {
        _owner = owner;
    }

    public override Type Tick()
    {
        if (_owner.onCooldown) //Time to stop and ready to attack player (Stop and face player without attack drawn)
        {
            _owner.FacePlayer();

            _owner.cooldownTimer += Time.deltaTime;

            if (_owner.cooldownTimer >= 2f)
            {
                _owner.cooldownTimer = 0f;
                _owner.onCooldown = false;
                _owner.toAttack = true;

                _owner.PrepareAttackArea();
            }
        }

        if (_owner.toAttack) //Draw area while facing the player before moving inside said area (Preparation)
        {
            _owner.waitToChargeTimer += Time.deltaTime;

            if (_owner.waitToChargeTimer >= _owner.requiredTimeToAttack)
            {
                _owner.waitToChargeTimer = 0f;
                _owner.toAttack = false;
                _owner.attacking = true;

                _owner.StartDashAttack();
                PlayAttackFeedback();
            }
        }

        if (_owner.attacking) // Attack
        {
            _owner.CheckDashFinished();

            _owner.attackTimer += Time.deltaTime;

            if (_owner.attackTimer >= 2f)
            {
                _owner.attackTimer = 0f;
                _owner.attacking = false;

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

        _owner.onCooldown = true;
        _owner.toAttack = false;
        _owner.attacking = false;

        _owner.cooldownTimer = 0f;
        _owner.waitToChargeTimer = 0f;
        _owner.attackTimer = 0f;

        _owner.OnAttackStateEnter();
    }

    public override void OnExit(BaseState newState)
    {
        _owner.OnAttackStateExit();
    }
}