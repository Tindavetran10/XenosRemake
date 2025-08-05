using UnityEngine;

namespace Scripts
{
    public class EnemyDeathState : EnemyState
    {
        public EnemyDeathState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            StateMachine.SwitchOffStateMachine();
            Debug.Log("Enter Death state");
        }
    }
}