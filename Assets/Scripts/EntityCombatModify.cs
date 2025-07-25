using UnityEngine;

namespace Scripts
{
    public class EntityCombatModify : MonoBehaviour
    {
        [Header("Target detection")]
        [SerializeField] public LayerMask targetLayer;
        [SerializeField] public Transform targetCheck;
        [SerializeField] public AttackDataSO[] attackDataArray;
        
        // Cache the entity reference for facing direction
        private Entity _entity;

        private void Awake()
        {
            _entity = GetComponent<Entity>();
            if (_entity == null) Debug.LogError("EntityCombat requires an Entity component!");
        }

        public void PerformAttack(int attackIndex = 0)
        {
            if (attackDataArray == null || attackDataArray.Length == 0)
            {
                Debug.LogWarning("No attack data configured!");
                return;
            }
            
            // Clamp index to valid range
            int index = Mathf.Clamp(attackIndex, 0, attackDataArray.Length - 1);
            var attackData = attackDataArray[index];
            
            if (attackData == null)
            {
                Debug.LogWarning($"Attack data at index {index} is null!");
                return;
            }
            
            ExecuteAttack(attackData, index);
        }
        
        private void ExecuteAttack(AttackDataSO attackData, int attackIndex)
        {
            // Calculate attack position with a facing-direction-aware offset
            var attackPosition = GetAttackPosition(attackData);
            
            // Calculate attack angle based on the facing direction
            var attackAngle = GetAttackAngle(attackData);
            
            // Get all colliders hit by this attack
            Collider2D[] targets = GetRectangleColliders(attackPosition, attackData, attackAngle);
            
            // Process each target
            foreach (var target in targets)
            {
                Debug.Log($"Hit {target.gameObject.name} with rectangle attack {attackIndex}");
                // Here you can add damage dealing, effects, etc.
            }
        }
        
        private Vector3 GetAttackPosition(AttackDataSO attackData)
        {
            Vector2 facingAwareOffset;
    
            if (attackData.offsetInFrontOfCharacter)
            {
                // Ensure attack is always in front of character
                facingAwareOffset = new Vector2(
                    Mathf.Abs(attackData.offset.x) * _entity.FacingDirection,
                    attackData.offset.y
                );
            }
            else
            {
                // Use raw offset with facing direction
                facingAwareOffset = new Vector2(
                    attackData.offset.x * _entity.FacingDirection,
                    attackData.offset.y
                );
            }
    
            return targetCheck.position + (Vector3)facingAwareOffset;
        }

        private float GetAttackAngle(AttackDataSO attackData)
        {
            if (attackData.flipAngleWithFacing && !_entity.FacingRight)
                return -attackData.angle;
            return attackData.angle;
        }
        
        private Collider2D[] GetRectangleColliders(Vector3 position, AttackDataSO attackData, float angle) =>
            // Use Physics2D.OverlapBoxAll for rectangle collision detection with the calculated angle
            Physics2D.OverlapBoxAll(position, attackData.size, angle, targetLayer);

        private void OnDrawGizmos()
        {
            if (attackDataArray == null || targetCheck == null || _entity == null) return;
            
            // Draw all attack rectangles with different colors
            for (var i = 0; i < attackDataArray.Length; i++)
            {
                var attackData = attackDataArray[i];
                if (attackData == null) continue;
                
                // Use the custom gizmo color or generate one based on index
                Gizmos.color = attackData.gizmoColor != Color.clear 
                    ? attackData.gizmoColor 
                    : Color.HSVToRGB(i / (float)attackDataArray.Length, 1f, 0.8f);
                
                // Get facing-aware position and angle
                Vector3 attackPosition = GetAttackPosition(attackData);
                var attackAngle = GetAttackAngle(attackData);
                
                DrawRectangleGizmo(attackPosition, attackData, attackAngle);
                
                // Draw a small arrow to show facing direction
                DrawFacingIndicator(attackPosition);
            }
        }
        
        private void DrawRectangleGizmo(Vector3 position, AttackDataSO attackData, float angle)
        {
            // Save the original matrix
            var oldMatrix = Gizmos.matrix;
            
            // Apply rotation and position
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(0, 0, angle), Vector3.one);
            
            // Draw the rectangle (wireframe)
            Gizmos.DrawWireCube(Vector3.zero, attackData.size);
            
            // Also draw a semi-transparent filled rectangle for better visualization
            Color fillColor = Gizmos.color;
            fillColor.a = 0.2f;
            Gizmos.color = fillColor;
            Gizmos.DrawCube(Vector3.zero, attackData.size);
            
            // Restore the original matrix
            Gizmos.matrix = oldMatrix;
        }
        
        private void DrawFacingIndicator(Vector3 position)
        {
            // Draw a small arrow showing the facing direction
            Gizmos.color = Color.yellow;
            var arrowEnd = position + Vector3.right * (_entity.FacingDirection * 0.5f);
            Gizmos.DrawLine(position, arrowEnd);
            
            // Draw arrowhead
            Vector3 arrowHead1 = arrowEnd + Vector3.up * 0.1f + Vector3.left * (_entity.FacingDirection * 0.1f);
            Vector3 arrowHead2 = arrowEnd + Vector3.down * 0.1f + Vector3.left * (_entity.FacingDirection * 0.1f);
            Gizmos.DrawLine(arrowEnd, arrowHead1);
            Gizmos.DrawLine(arrowEnd, arrowHead2);
        }
    }
}