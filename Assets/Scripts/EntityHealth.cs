using UnityEngine;

namespace Scripts
{
    public class EntityHealth : MonoBehaviour
    {
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected bool isDead;

        public virtual void TakeDamage(float damage, Transform damageDealer = null)
        {
            if(isDead) return;
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