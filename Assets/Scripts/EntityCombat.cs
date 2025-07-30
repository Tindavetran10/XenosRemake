using UnityEngine;

namespace Scripts
{
    public class EntityCombat : MonoBehaviour
    {
        public float damage = 10;
        
        [Header("Target detection")]
        [SerializeField] public LayerMask targetLayer;
        [SerializeField] public float targetDetectionRadius = 1f;
        [SerializeField] public Transform targetCheck;

        public void PerformAttack()
        {
            foreach (var target in GetDetectedColliders())
            {
                EntityHealth targetHealth = target.GetComponent<EntityHealth>();
                
                targetHealth?.TakeDamage(damage);
            }
        }
        
        private Collider2D[] GetDetectedColliders() => 
            Physics2D.OverlapCircleAll(targetCheck.position, targetDetectionRadius, targetLayer);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetCheck.position, targetDetectionRadius);
        }
    }
}