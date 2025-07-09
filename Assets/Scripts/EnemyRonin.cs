using UnityEngine;

namespace Scripts
{
    public class EnemyRonin : Enemy
    {
        protected override void Awake()
        {
            base.Awake();
            IdleState = new EnemyIdleState(this, StateMachine, "idle");
            MoveState = new EnemyMoveState(this, StateMachine, "move");
        }

        protected override void Start()
        {
            base.Start();
            StateMachine.Initialize(IdleState);
        }
    }
}