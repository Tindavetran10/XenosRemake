
public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Input.Disable();
        Rb.simulated = false;
    }
}
