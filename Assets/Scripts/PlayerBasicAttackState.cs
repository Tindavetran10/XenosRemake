using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerBasicAttackState : EntityState
    {
        private static readonly int BasicAttackIndex = Animator.StringToHash("basicAttackIndex");
        private float _attackVelocityTimer;
        
        private const int FirstComboIndex = 1; // We start combo Index with 1, this parameter is used in the Animator
        private const int ComboLimit = 3;
        private int _comboIndex = 1;

        public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : 
            base(player, stateMachine, animBoolName) {}

        public override void Enter()
        {
            base.Enter();
            
            HandleComboLimit();
            Debug.Log($"Entering attack state with combo index: {_comboIndex}");
            
            VelocityTriggerCalled = false;
            StopVelocityTriggerCalled = false;
            SkipAnimationTriggerCalled = false;
            
            Anim.SetInteger(BasicAttackIndex, _comboIndex);
        }
        
        public override void Update()
        {
            base.Update();
            HandleAttackVelocity();
            
            if(SkipAnimationTriggerCalled) 
                TriggerCalled = true;
            
            if(TriggerCalled)
                StateMachine.ChangeState(Player.idleState);
        }
        
        public override void Exit()
        {
            base.Exit();
            _comboIndex++;
            
            // Remember the time when we attacked
        }

        private void HandleComboLimit()
        {
            if(_comboIndex > ComboLimit)
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
            _attackVelocityTimer = Player.attackVelocityDuration;
            Player.SetVelocityY(Player.attackVelocity.x * Player.facingDirection, Player.attackVelocity.y);
        }
    }
}