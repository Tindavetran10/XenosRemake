using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    private Player _player;
    
    public void Awake() => _player = GetComponentInParent<Player>();
    public void CurrentStateTrigger() => _player.CallAnimationTrigger();
    public void CurrentVelocityStateTrigger() => _player.CallVelocityAnimationTrigger();
    public void StopVelocityStateTrigger() => _player.CallStopVelocityAnimationTrigger();
}
