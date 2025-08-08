namespace Scripts.AnimationTrigger
{
    public class PlayerAnimationTrigger : EntityAnimationTrigger
    {
        private global::Player _player;
        private PlayerCombat _playerCombat;

        public override void Awake()
        {
            _player = GetComponentInParent<global::Player>();
            _playerCombat = GetComponentInParent<PlayerCombat>();
        }

        public override void CurrentStateTrigger() => _player.CurrentStateAnimationTrigger();
        public override void CurrentVelocityStateTrigger() => _player.CallVelocityAnimationTrigger();
        public override void StopVelocityStateTrigger() => _player.CallStopVelocityAnimationTrigger();
        public override void AttackTrigger() => _playerCombat.PerformAttack();
        public override void DeathTrigger() => _player.CallDeathAnimationTrigger();
        
        // Updated to support attack index
        /*public override void AttackTrigger() => AttackTrigger(0);
        public override void AttackTrigger(int attackIndex) => _playerCombat.PerformAttack(attackIndex);*/

        public void SkipCurrentStateTrigger() => _player.SkipCallAnimationTrigger();
        
        public void CheckIfShouldFlipTrigger() => _player.CheckIfShouldFlipTrigger();
    }
}