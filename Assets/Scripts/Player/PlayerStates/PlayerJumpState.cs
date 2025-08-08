
/// <summary>
/// Handles player state when he's jumping
/// </summary>
public class PlayerJumpState : PlayerAirState
{
    private bool _jumpInputReleased;

    public PlayerJumpState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        _jumpInputReleased = false;
        Player.currentJumps++;
        // Make object go up, increase Y velocity
        Player.SetVelocityY(Rb.linearVelocity.x, Player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        if (Rb.linearVelocity.y < 0)
        {
            StateMachine.ChangeState(Player.FallState);
            return;
        }

        if (_jumpInputReleased || Input.Player.Jump.IsPressed())
            return;

        _jumpInputReleased = true;
        if (Rb.linearVelocity.y > 0)
            Player.SetVelocityY(Rb.linearVelocity.x, Rb.linearVelocity.y * 0.5f);

    }
}
