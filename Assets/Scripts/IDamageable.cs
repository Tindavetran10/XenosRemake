using UnityEngine;

namespace Scripts
{
    public interface IDamageable
    {
        public void TakeDamage(float damage, Transform damageDealer = null);
    }
}