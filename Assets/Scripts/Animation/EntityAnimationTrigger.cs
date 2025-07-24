using Scripts;
using UnityEngine;

public class EntityAnimationTrigger : MonoBehaviour
{
    private Entity _entity;
    private EntityCombat _entityCombat;
    private bool _shouldSkipRemainingAnimation;
    
    public virtual void Awake()
    {
        _entity = GetComponentInParent<Entity>();
        _entityCombat = GetComponentInParent<EntityCombat>();
    }

    public virtual void CurrentStateTrigger() => _entity.CurrentStateAnimationTrigger();
    public virtual void CurrentVelocityStateTrigger() => _entity.CallVelocityAnimationTrigger();
    public virtual void StopVelocityStateTrigger() => _entity.CallStopVelocityAnimationTrigger();
    public virtual void AttackTrigger() => _entityCombat.PerformAttack();
}
