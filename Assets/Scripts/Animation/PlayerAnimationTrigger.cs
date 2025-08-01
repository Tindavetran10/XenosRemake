﻿namespace Scripts.Animation
{
    public class PlayerAnimationTrigger : EntityAnimationTrigger
    {
        private Player _player;
        private PlayerCombat _playerCombat;

        public override void Awake()
        {
            _player = GetComponentInParent<Player>();
            _playerCombat = GetComponentInParent<PlayerCombat>();
        }

        public override void CurrentStateTrigger() => _player.CurrentStateAnimationTrigger();
        public override void CurrentVelocityStateTrigger() => _player.CallVelocityAnimationTrigger();
        public override void StopVelocityStateTrigger() => _player.CallStopVelocityAnimationTrigger();
        public override void AttackTrigger() => _playerCombat.PerformAttack();
        
        // Updated to support attack index
        /*public override void AttackTrigger() => AttackTrigger(0);
        public override void AttackTrigger(int attackIndex) => _playerCombat.PerformAttack(attackIndex);*/

        public void SkipCurrentStateTrigger() => _player.SkipCallAnimationTrigger();
        
        public void CheckIfShouldFlipTrigger() => _player.CheckIfShouldFlipTrigger();
    }
}