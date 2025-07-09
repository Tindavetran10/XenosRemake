using UnityEngine;

namespace Scripts
{
    public class EnemyState : EntityState
    {
        public Enemy Enemy;
        
        public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
        {
            Enemy = enemy;

            Rb = enemy.rb;
            Anim = enemy.animator;
        }
    }
}