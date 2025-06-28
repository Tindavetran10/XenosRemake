using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerBasicAttackState : EntityState
    {
        private float _attackVelocityTimer;
        
        public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : 
            base(player, stateMachine, animBoolName) {}

        public override void Enter()
        {
            base.Enter();
            VelocityTriggerCalled = false;
            StopVelocityTriggerCalled = false;
        }
        
        public override void Update()
        {
            base.Update();
            HandleAttackVelocity();
            
            if(TriggerCalled)
                StateMachine.ChangeState(Player.idleState);
        }

        private void HandleAttackVelocity()
        {
            _attackVelocityTimer -= Time.deltaTime;
            
            if(VelocityTriggerCalled)
                GenerateAttackVelocity();
            
            if(StopVelocityTriggerCalled)
                Player.SetVelocityY(0, Rb.linearVelocity.y);
            
            if(_attackVelocityTimer < 0)
                Player.SetVelocityY(0, Rb.linearVelocity.y);
        }

        private void GenerateAttackVelocity()
        {
            _attackVelocityTimer = Player.attackVelocityDuration;
            Player.SetVelocityY(Player.attackVelocity.x * Player.facingDirection, Player.attackVelocity.y);
        }
    }
}