using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerGroundState : EntityState
    {
        protected PlayerGroundState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Update()
        {
            base.Update();
            if(Input.Player.Jump.WasPerformedThisFrame())
                Debug.Log("Jump");
        }
    }
}