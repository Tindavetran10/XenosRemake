using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerAirState : EntityState
    {
        protected PlayerAirState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Enter()
        {
            base.Enter();
        }
        
        public override void Update()
        {
            base.Update();
            
            if(Player.moveInput.x != 0)
                Player.SetVelocityX(Player.moveInput.x * Player.moveSpeed * Player.inAirMoveMultiplier, Rb.linearVelocity.y);
        }
    }
}