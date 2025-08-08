namespace Scripts
{
    public class EnemyRonin : Enemy, ICounterable
    {
        protected override void Awake()
        {
            base.Awake();
            IdleState = new EnemyIdleState(this, StateMachine, "idle");
            MoveState = new EnemyMoveState(this, StateMachine, "move");
            AttackState = new EnemyAttackState(this, StateMachine, "attack");
            BattleState = new EnemyBattleState(this, StateMachine, "battle");
            DeathState = new EnemyDeathState(this, StateMachine, "death");
            StunnedState = new EnemyStunnedState(this, StateMachine, "stunned");
        }

        protected override void Start()
        {
            base.Start();
            StateMachine.Initialize(IdleState);
        }
        
        public void HandleCounter()
        {
            if (canBeStunned == false)
                return;
            
            StateMachine.ChangeState(StunnedState);
        }
    }
}