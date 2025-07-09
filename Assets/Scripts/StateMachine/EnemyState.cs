using UnityEngine;

namespace Scripts
{
    public class EnemyState : EntityState
    {
        private static readonly int MoveAnimSpeedMultiplier = Animator.StringToHash("moveAnimSpeedMultiplier");
        protected readonly Enemy Enemy;

        protected EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
        {
            Enemy = enemy;

            Rb = enemy.rb;
            Anim = enemy.animator;
        }

        public override void Enter()
        {
            base.Enter();
            Anim.SetFloat(MoveAnimSpeedMultiplier, Enemy.moveAnimSpeedMultiplier);
        }
    }
}