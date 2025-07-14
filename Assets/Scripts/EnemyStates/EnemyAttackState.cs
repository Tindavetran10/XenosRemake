using UnityEngine;

namespace Scripts
{
    public class EnemyAttackState : EnemyState
    {
        public EnemyAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }

        public override void Update()
        {
            base.Update();
            
            if(TriggerCalled)
                StateMachine.ChangeState(Enemy.IdleState);;
        }
    }
}