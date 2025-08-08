using UnityEngine;

public class EnemyState : EntityState
{
    private static readonly int MoveAnimSpeedMultiplier = Animator.StringToHash("moveAnimSpeedMultiplier");
    private static readonly int BattleAnimSpeedMultiplier = Animator.StringToHash("battleAnimSpeedMultiplier");
    private static readonly int XVelocity = Animator.StringToHash("xVelocity");
    protected readonly Enemy Enemy;

    protected EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        Enemy = enemy;

        Rb = enemy.Rb;
        Anim = enemy.Animator;
        SpriteRenderer = enemy.SpriteRenderer;
    }

    protected override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        
        var battleAnimSpeedMultiplier = Enemy.battleMoveSpeed / Enemy.moveSpeed;
        
        Anim.SetFloat(MoveAnimSpeedMultiplier, Enemy.moveAnimSpeedMultiplier);
        Anim.SetFloat(BattleAnimSpeedMultiplier, battleAnimSpeedMultiplier);
        Anim.SetFloat(XVelocity, Rb.linearVelocity.x);
    }
}
