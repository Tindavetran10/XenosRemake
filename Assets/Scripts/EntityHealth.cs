using System;
using UnityEngine;

namespace Scripts
{
    public class EntityHealth : MonoBehaviour
    {
        private EntityVFX _entityVFX;
        
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected bool isDead;

        private void Awake() => _entityVFX = GetComponent<EntityVFX>();

        public virtual void TakeDamage(float damage, Transform damageDealer = null)
        {
            if(isDead) return;
            _entityVFX?.PlayOnDamageVFX();
            
            ReducedHealth(damage);
        }
        
        protected void ReducedHealth(float damage)
        {
            maxHealth -= damage;

            if (maxHealth < 0)
                Die();
        }

        private void Die()
        {
            isDead = true;
            Debug.Log("Dead");
        }
    }
}