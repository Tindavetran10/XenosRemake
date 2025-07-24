using UnityEngine;

namespace Scripts
{
    public class EntityCombat : MonoBehaviour
    {
        [Header("Target detection")]
        [SerializeField] public LayerMask targetLayer;
        [SerializeField] public float targetDetectionRadius = 1f;
        [SerializeField] public Transform targetCheck;

        public void PerformAttack()
        {
            foreach (var target in GetDetectedColliders())
            {
                Debug.Log(target.gameObject.name);
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