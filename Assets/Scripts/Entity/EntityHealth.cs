using UnityEngine;

public class EntityHealth : MonoBehaviour, IDamageable
{
    private Entity _entity;
    private EntityVFX _entityVFX;

    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected bool isDead;

    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new(1.5f, 0f);
    [SerializeField] private Vector2 heavyKnockbackPower = new(10f, 0f);
    [SerializeField] private float knockbackDuration = .2f;
    [SerializeField] private float heavyKnockbackDuration = .5f;

    [Header("On Heavy Damage")]
    [SerializeField] private float heavyDamageThreshold = 0.3f; // Percentage of health you should lose to consider damage as heavy

    private void Awake()
    {
        _entity = GetComponent<Entity>();
        _entityVFX = GetComponent<EntityVFX>();

        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage, Transform damageDealer = null)
    {
        if (isDead) return;

        var knockback = CalculateKnockback(damage, damageDealer);
        var duration = CalculateDuration(damage);

        _entity.ReceiveKnockback(knockback, duration);
        _entityVFX?.PlayOnDamageVFX();
        ReducedHealth(damage);
    }

    protected void ReducedHealth(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        _entity.EntityDeath();
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        var direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        var knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;
        knockback.x *= direction;
        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    private bool IsHeavyDamage(float damage) => damage / maxHealth > heavyDamageThreshold;
}
