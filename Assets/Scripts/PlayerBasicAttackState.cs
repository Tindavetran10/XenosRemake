using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerBasicAttackState : EntityState
    {
        private static readonly int BasicAttackIndex = Animator.StringToHash("basicAttackIndex");
        private float _attackVelocityTimer;
        
        private const int FirstComboIndex = 1; // We start combo Index with 1, this parameter is used in the Animator
        private int _attackDirection;
        private readonly int _comboLimit = 3;
        private int _comboIndex = 1;
        
        private float _lastTimeAttacked;
        private bool _comboAttackQueued;
        private bool _shouldSkipAnimation;

        public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) :
            base(player, stateMachine, animBoolName)
        {
            if (!_comboLimit.Equals(Player.attackVelocity.Length))
            {
                Debug.LogWarning("I've adjusted combo limit, according to attack velocity array length.");
                _comboLimit = Player.attackVelocity.Length;
            }
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
            
            _attackDirection = Player.moveInput.x != 0 ? (int)Player.moveInput.x : Player.facingDirection; 
            
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
            if(SkipTriggerCalled && _shouldSkipAnimation) HandleSkipStateExit();
            
            // Handle normal animation end
            if (TriggerCalled) HandleStateExit();
        }
        
        public override void Exit()
        {
            base.Exit();
            _comboIndex++;
            
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
            else StateMachine.ChangeState(Player.idleState);
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
            
            if(StopVelocityTriggerCalled || _attackVelocityTimer < 0)
                Player.SetVelocityY(0, Rb.linearVelocity.y);
        }

        private void ApplyAttackVelocity()
        {
            // Clamp the index to a valid range
            int index = Mathf.Clamp(_comboIndex - 1, 0, Player.attackVelocity.Length - 1);
            var attackVelocity = Player.attackVelocity[index];

            _attackVelocityTimer = Player.attackVelocityDuration;
            Player.SetVelocityY(attackVelocity.x * _attackDirection, attackVelocity.y);
        }
        #endregion
    }
}