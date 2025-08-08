
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private static readonly int AttackIndex = Animator.StringToHash("attackIndex");
    private float _lastTimeAttacked;
    private float _attackVelocityTimer;

    public EnemyAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        HandleComboLimit();
        Anim.SetInteger(AttackIndex, ComboIndex);

        // Reset velocity triggers for each attack
        VelocityTriggerCalled = false;
        StopVelocityTriggerCalled = false;
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (TriggerCalled)
            StateMachine.ChangeState(Enemy.BattleState);
    }

    public override void Exit()
    {
        base.Exit();
        ComboIndex++;
        _lastTimeAttacked = Time.time;
    }

    private void HandleComboLimit()
    {
        if (ComboIndex > ComboLimit || Time.time > _lastTimeAttacked + Enemy.comboResetTime)
            ComboIndex = FirstComboIndex;
    }

    private void HandleAttackVelocity()
    {
        _attackVelocityTimer -= Time.deltaTime;

        if (VelocityTriggerCalled)
            ApplyAttackVelocity();

        if (StopVelocityTriggerCalled || _attackVelocityTimer < 0)
            Enemy.SetVelocityY(0, Rb.linearVelocity.y);
    }

    private void ApplyAttackVelocity()
    {
        // Clamp the index to a valid range
        int index = Mathf.Clamp(ComboIndex - 1, 0, Enemy.attackVelocity.Length - 1);
        var attackVelocity = Enemy.attackVelocity[index];

        _attackVelocityTimer = Enemy.attackVelocityDuration; // ✅ FIXED: Use Enemy instead of Player
        Enemy.SetVelocityY(attackVelocity.x * Enemy.FacingDirection, attackVelocity.y);
    }
}
