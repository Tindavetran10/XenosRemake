
using UnityEngine;

public class EnemyBattleState : EnemyState
{
    private Transform _playerTransform;
    private float _lastTimeWasInBattle;
    public EnemyBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        UpdateBattleTimer();

        _playerTransform ??= Enemy.GetPlayerReference();

        if (ShouldRetreat())
        {
            Rb.linearVelocity = new Vector2(Enemy.retreatVelocity.x * -DirectionToPlayer(), Rb.linearVelocity.y);
            Enemy.HandleFlip(DirectionToPlayer());
        }
    }

    public override void Update()
    {
        base.Update();

        if (Enemy.PlayerDetected())
            UpdateBattleTimer();

        if (BattleTimeIsOver())
            StateMachine.ChangeState(Enemy.IdleState);

        if (WithinAttackRange() && Enemy.PlayerDetected())
            StateMachine.ChangeState(Enemy.AttackState);
        else Enemy.SetVelocityX(Enemy.battleMoveSpeed * DirectionToPlayer(), Rb.linearVelocity.y);

    }

    private void UpdateBattleTimer() => _lastTimeWasInBattle = Time.time;
    private bool BattleTimeIsOver() => Time.time > _lastTimeWasInBattle + Enemy.battleTimeDuration;

    private bool WithinAttackRange() => DistanceToPlayer() < Enemy.attackDistance;
    private bool ShouldRetreat() => DistanceToPlayer() < Enemy.minRetreatDistance;

    private float DistanceToPlayer()
    {
        if (_playerTransform == null)
            return float.MaxValue;
        return Mathf.Abs(_playerTransform.position.x - Enemy.transform.position.x);
    }

    private int DirectionToPlayer()
    {
        if (_playerTransform == null)
            return 0;
        return _playerTransform.position.x > Enemy.transform.position.x ? 1 : -1;
    }
}
