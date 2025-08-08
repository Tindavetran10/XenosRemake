using UnityEngine;


public class PlayerBasicAttackState : PlayerState
{
    private static readonly int BasicAttackIndex = Animator.StringToHash("basicAttackIndex");
    private float _attackVelocityTimer;

    private float _lastTimeAttacked;
    private bool _comboAttackQueued;
    private bool _shouldSkipAnimation;

    public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) :
        base(player, stateMachine, animBoolName)
    {
        if (ComboLimit.Equals(Player.attackVelocity.Length)) return;
        Debug.LogWarning("I've adjusted combo limit, according to attack velocity array length.");
        ComboLimit = Player.attackVelocity.Length;
    }

    public override void Enter()
    {
        base.Enter();

        HandleComboLimit();

        VelocityTriggerCalled = false;
        StopVelocityTriggerCalled = false;

        _comboAttackQueued = false;
        _shouldSkipAnimation = false;

        // Set the current combo index in the animator
        Anim.SetInteger(BasicAttackIndex, ComboIndex);
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        // Check for skip condition when attack is pressed
        if (Input.Player.Attack.WasPressedThisFrame())
        {
            if (_shouldSkipAnimation)
                QueueNextAttack();
            else _shouldSkipAnimation = true;
        }

        // Handle state transitions
        if (SkipTriggerCalled && _shouldSkipAnimation) HandleSkipStateExit();

        // Handle normal animation end
        if (TriggerCalled) HandleStateExit();
    }

    public override void Exit()
    {
        base.Exit();
        ComboIndex++;

        // Remember the time when we attacked
        _lastTimeAttacked = Time.time;
    }

    #region Attack's State Methods
    private void HandleSkipStateExit()
    {
        if (!_comboAttackQueued) return;
        // Reset animation and queue the next attack
        Anim.SetBool(AnimBoolName, false);
        Player.EnterAttackStateWithDelay();
    }

    private void HandleStateExit()
    {
        if (_comboAttackQueued)
        {
            Anim.SetBool(AnimBoolName, false);
            Player.EnterAttackStateWithDelay();
        }
        else StateMachine.ChangeState(Player.IdleState);
    }

    private void QueueNextAttack()
    {
        if (ComboIndex < ComboLimit)
            _comboAttackQueued = true;
    }

    private void HandleComboLimit()
    {
        if (ComboIndex > ComboLimit || Time.time > _lastTimeAttacked + Player.comboResetTime)
            ComboIndex = FirstComboIndex;
    }

    private void HandleAttackVelocity()
    {
        _attackVelocityTimer -= Time.deltaTime;

        if (VelocityTriggerCalled)
            ApplyAttackVelocity();

        if (StopVelocityTriggerCalled || _attackVelocityTimer < 0)
            Player.SetVelocityY(0, Rb.linearVelocity.y);
    }

    private void ApplyAttackVelocity()
    {
        // Clamp the index to a valid range
        int index = Mathf.Clamp(ComboIndex - 1, 0, Player.attackVelocity.Length - 1);
        var attackVelocity = Player.attackVelocity[index];

        _attackVelocityTimer = Player.attackVelocityDuration;
        Player.SetVelocityY(attackVelocity.x * Player.FacingDirection, attackVelocity.y);
    }

    // Add this method to allow velocity update on flip
    public void UpdateAttackVelocity()
    {
        // Clamp the index to a valid range
        int index = Mathf.Clamp(ComboIndex - 1, 0, Player.attackVelocity.Length - 1);
        var attackVelocity = Player.attackVelocity[index];
        Player.SetVelocityY(attackVelocity.x * Player.FacingDirection, attackVelocity.y);
    }
    #endregion
}
