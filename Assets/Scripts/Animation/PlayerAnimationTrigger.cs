namespace Scripts.Animation
{
    public class PlayerAnimationTrigger : EntityAnimationTrigger
    {
        private Player _player;

        public override void Awake() => _player = GetComponentInParent<Player>();
        public override void CurrentStateTrigger() => _player.CurrentStateAnimationTrigger();
        public override void CurrentVelocityStateTrigger() => _player.CallVelocityAnimationTrigger();
        public override void StopVelocityStateTrigger() => _player.CallStopVelocityAnimationTrigger();

        public void SkipCurrentStateTrigger() => _player.SkipCallAnimationTrigger();
        
        public void CheckIfShouldFlipTrigger() => _player.CheckIfShouldFlipTrigger();
    }
}