public class EnemyGroundedState : EnemyState
{
    protected EnemyGroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // If enemy detects Player
        // State machine switch to battle state
        if (Enemy.PlayerDetected())
            StateMachine.ChangeState(Enemy.BattleState);
    }
}
