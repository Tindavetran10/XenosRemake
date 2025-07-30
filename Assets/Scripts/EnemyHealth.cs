using UnityEngine;

namespace Scripts
{
    public class EnemyHealth : EntityHealth
    {
        private Enemy Enemy => GetComponent<Enemy>();
        
        public override void TakeDamage(float damage, Transform damageDealer = null)
        {
            if(damageDealer!.GetComponent<Player>() != null)
                Enemy.TryEnterBattleState(damageDealer);
            
            base.TakeDamage(damage, damageDealer);
        }
    }
}