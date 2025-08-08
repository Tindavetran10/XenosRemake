using UnityEngine;

namespace Scripts
{
    public class EnemyStunnedState : EnemyState
    {
        private EnemyVFX vfx;
        
        public EnemyStunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
            vfx = Enemy.GetComponent<EnemyVFX>();
        }
        
        public override void Enter()
        {
            base.Enter();
            vfx.EnableAttackAlert(false);
            Enemy.EnableCounterWindow(false);
            StateTimer = Enemy.stunnedDuration;
            Rb.linearVelocity = new Vector2(Enemy.stunnedVelocity.x * -Enemy.FacingDirection, Enemy.stunnedVelocity.y);
        }
        
        public override void Update()
        {
            base.Update();
            if(StateTimer < 0)
                StateMachine.ChangeState(Enemy.IdleState);
        }
    }
}