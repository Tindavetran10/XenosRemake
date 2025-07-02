using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    private Player _player;
    private bool _shouldSkipRemainingAnimation;
    
    public void Awake() => _player = GetComponentInParent<Player>();
    public void CurrentStateTrigger() => _player.CallAnimationTrigger();
    public void SkipCurrentStateTrigger() => _player.SkipCallAnimationTrigger();
    public void CurrentVelocityStateTrigger() => _player.CallVelocityAnimationTrigger();
    public void StopVelocityStateTrigger() => _player.CallStopVelocityAnimationTrigger();
    public void CheckIfShouldFlipTrigger() => _player.CheckIfShouldFlipTrigger();
}
