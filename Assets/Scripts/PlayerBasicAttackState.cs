using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerBasicAttackState : EntityState
    {
        private static readonly int BasicAttackIndex = Animator.StringToHash("basicAttackIndex");
        private float _attackVelocityTimer;
        
        private const int FirstComboIndex = 1; // We start combo Index with 1, this parameter is used in the Animator
        private int _comboLimit = 3;
        private int _comboIndex = 1;
        
        private float _lastTimeAttacked;
        private bool _comboAttackQueued;
        private bool _shouldSkipAnimation;

        public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) :
            base(player, stateMachine, animBoolName)
        {
            if (_comboLimit.Equals(Player.attackVelocity.Length)) return;
            Debug.LogWarning("I've adjusted combo limit, according to attack velocity array length.");
            _comboLimit = Player.attackVelocity.Length;
        }

        public override void Enter()
        {
            base.Enter();
            
            HandleComboLimit();
            Debug.Log($"Entering attack state with combo index: {_comboIndex}");
            
            VelocityTriggerCalled = false;
            StopVelocityTriggerCalled = false;
            
            _comboAttackQueued = false;
            _shouldSkipAnimation = false;
            
            // Set the current combo index in the animator
            Anim.SetInteger(BasicAttackIndex, _comboIndex);
        }
        
        public override void Update()
        {
            base.Update();
            HandleAttackVelocity();
            
            // Check for skip condition when attack is pressed
            if (Input.Player.Attack.WasPressedThisFrame())
            {
                if(_shouldSkipAnimation)
                    QueueNextAttack();
                else _shouldSkipAnimation = true;
            }

            // Handle state transitions
            if(SkipTriggerCalled && _shouldSkipAnimation)
            {
                if(_comboAttackQueued)
                {
                    // Reset animation and queue the next attack
                    Anim.SetBool(AnimBoolName, false);
                    Player.EnterAttackStateWithDelay();
                    return;
                }
                StateMachine.ChangeState(Player.idleState);
                return;
            }
            
            // Handle normal animation end
            if (TriggerCalled)
            {
                if (_comboAttackQueued)
                {
                    Anim.SetBool(AnimBoolName, false);
                    Player.EnterAttackStateWithDelay();
                }
                else StateMachine.ChangeState(Player.idleState);
            }
        }
        
        public override void Exit()
        {
            base.Exit();
            _comboIndex++;
            
            // Remember the time when we attacked
            _lastTimeAttacked = Time.time;
        }

        private void QueueNextAttack()
        {
            if(_comboIndex < _comboLimit)
                _comboAttackQueued = true;
        }

        private void HandleComboLimit()
        {
            if(_comboIndex > _comboLimit || Time.time > _lastTimeAttacked + Player.comboResetTime)
                _comboIndex = FirstComboIndex;
        }

        private void HandleAttackVelocity()
        {
            _attackVelocityTimer -= Time.deltaTime;
            
            if(VelocityTriggerCalled)
                ApplyAttackVelocity();
            
            if(StopVelocityTriggerCalled)
                Player.SetVelocityY(0, Rb.linearVelocity.y);
            
            if(_attackVelocityTimer < 0)
                Player.SetVelocityY(0, Rb.linearVelocity.y);
        }

        private void ApplyAttackVelocity()
        {
            Vector2 attackVelocity = Player.attackVelocity[_comboIndex - 1];
            
            _attackVelocityTimer = Player.attackVelocityDuration;
            Player.SetVelocityY(attackVelocity.x * Player.facingDirection, attackVelocity.y);
        }
    }
}