using UnityEngine;

namespace Scripts
{
    public class EnemyAttackState : EnemyState
    {
        private static readonly int AttackIndex = Animator.StringToHash("attackIndex");
        private const int FirstComboIndex = 1; // We start combo Index with 1, this parameter is used in the Animator
        private const int ComboLimit = 3;
        private int _comboIndex = 1;
        
        private float _lastTimeAttacked;
        
        public EnemyAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            HandleComboLimit();
            Anim.SetInteger(AttackIndex, _comboIndex);
        }

        public override void Update()
        {
            base.Update();
            
            if(TriggerCalled)
                StateMachine.ChangeState(Enemy.BattleState);
        }

        public override void Exit()
        {
            base.Exit();
            _comboIndex++;
            _lastTimeAttacked = Time.time;
        }
        
        private void HandleComboLimit()
        {
            if(_comboIndex > ComboLimit || Time.time > _lastTimeAttacked + Enemy.comboResetTime)
                _comboIndex = FirstComboIndex;
        }
    }
}