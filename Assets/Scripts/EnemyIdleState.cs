using UnityEngine;

namespace Scripts
{
    public class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            StateTimer = Enemy.idleTime;
        }
        
        public override void Update()
        {
            base.Update();
            if(StateTimer < 0)
                StateMachine.ChangeState(Enemy.MoveState);
        }
    }
}