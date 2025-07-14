namespace Scripts.Animation
{
    public class PlayerAnimationTrigger : EntityAnimationTrigger
    {
        private Player _player;

        public override void Awake() => _player = GetComponentInParent<Player>();

        public override void CurrentStateTrigger() => _player.CallAnimationTrigger();

        public void SkipCurrentStateTrigger() => _player.SkipCallAnimationTrigger();
        public void CurrentVelocityStateTrigger() => _player.CallVelocityAnimationTrigger();
        public void StopVelocityStateTrigger() => _player.CallStopVelocityAnimationTrigger();
        public void CheckIfShouldFlipTrigger() => _player.CheckIfShouldFlipTrigger();
    }
}