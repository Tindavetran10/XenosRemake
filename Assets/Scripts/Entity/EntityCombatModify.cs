using UnityEngine;


public class EntityCombatModify : MonoBehaviour
{
    [Header("Target detection")]
    [SerializeField] public LayerMask targetLayer;
    [SerializeField] public Transform targetCheck;
    [SerializeField] public AttackDataSO[] attackDataArray;

    // Cache the entity reference for the facing direction
    private Entity _entity;

    private void Awake()
    {
        _entity = GetComponent<Entity>();
        if (_entity == null)
        {
            Debug.LogError("EntityCombat requires an Entity component!");
        }
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
        Vector3 attackPosition = GetAttackPosition(attackData);

        // Calculate an attack angle based on the facing direction
        float attackAngle = GetAttackAngle(attackData);

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
            // Use raw offset with the facing direction
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

    private Collider2D[] GetRectangleColliders(Vector3 position, AttackDataSO attackData, float angle)
    {
        // Use Physics2D.OverlapBoxAll for rectangle collision detection with a calculated angle
        return Physics2D.OverlapBoxAll(position, attackData.size, angle, targetLayer);
    }

    // Utility methods
    public AttackDataSO GetAttackData(int attackIndex)
    {
        if (attackDataArray == null || attackIndex < 0 || attackIndex >= attackDataArray.Length)
            return null;
        return attackDataArray[attackIndex];
    }
    public int GetAttackCount() => attackDataArray?.Length ?? 0;
    public Entity GetEntity() => _entity;
}
