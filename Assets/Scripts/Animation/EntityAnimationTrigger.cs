using Scripts;
using UnityEngine;

public class EntityAnimationTrigger : MonoBehaviour
{
    private Entity _entity;
    private bool _shouldSkipRemainingAnimation;
    
    public virtual void Awake() => _entity = GetComponentInParent<Entity>();
    public virtual void CurrentStateTrigger() => _entity.CurrentStateAnimationTrigger();
    public virtual void CurrentVelocityStateTrigger() => _entity.CallVelocityAnimationTrigger();
    public virtual void StopVelocityStateTrigger() => _entity.CallStopVelocityAnimationTrigger();
}
