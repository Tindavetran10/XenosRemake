using UnityEngine;

namespace Scripts.PlayerStates
{
    public class PlayerCounterAttackState : PlayerState
    {
        public PlayerCounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            StateTimer = 1f;
        }
        
        public override void Update()
        {
            base.Update();
            if(StateTimer < 0)
                StateMachine.ChangeState(Player.IdleState);
        }
    }
}