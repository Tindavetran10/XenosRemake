using UnityEngine;

public class EnemyStunnedState : EnemyState
{
    private readonly EnemyVFX _vfx;

    public EnemyStunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        _vfx = Enemy.GetComponent<EnemyVFX>();
    }

    public override void Enter()
    {
        base.Enter();
        _vfx.EnableAttackAlert(false);
        Enemy.EnableCounterWindow(false);
        StateTimer = Enemy.stunnedDuration;
        Rb.linearVelocity = new Vector2(Enemy.stunnedVelocity.x * -Enemy.FacingDirection, Enemy.stunnedVelocity.y);
    }

    public override void Update()
    {
        base.Update();
        if (StateTimer < 0)
            StateMachine.ChangeState(Enemy.IdleState);
    }
}
