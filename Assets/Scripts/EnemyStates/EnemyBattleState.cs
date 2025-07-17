using UnityEngine;

namespace Scripts
{
    public class EnemyBattleState : EnemyState
    {
        public EnemyBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Battle State");
        }
        
    }
}