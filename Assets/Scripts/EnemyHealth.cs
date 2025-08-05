using UnityEngine;

namespace Scripts
{
    public class EnemyHealth : EntityHealth
    {
        private Enemy Enemy => GetComponent<Enemy>();
        
        public override void TakeDamage(float damage, Transform damageDealer = null)
        {
            base.TakeDamage(damage, damageDealer);
            
            if(isDead) return;
            
            if(damageDealer!.GetComponent<Player>() != null)
                Enemy.TryEnterBattleState(damageDealer);
            
        }
    }
}