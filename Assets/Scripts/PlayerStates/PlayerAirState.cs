namespace Scripts.PlayerStates
{
    /// <summary>
    /// Base class for all states in the Air, providing common state functionality
    /// </summary>
    public class PlayerAirState : PlayerState
    {
        protected PlayerAirState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Update()
        {
            base.Update();
            
            // Apply horizontal movement while in the air so that the player can move left or right
            if(Player.MoveInput.x != 0)
                Player.SetVelocityX(Player.MoveInput.x * 
                                    Player.moveSpeed * 
                                    Player.inAirMoveMultiplier, 
                    Rb.linearVelocity.y);
        }
    }
}