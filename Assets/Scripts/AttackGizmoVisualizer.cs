using UnityEngine;


/// <summary>
/// Handles visualization and debugging for EntityCombat attack data.
/// Keeps the main EntityCombat script clean by separating visualization logic.
/// </summary>
public class AttackGizmoVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    [SerializeField] private bool showAttackGizmos = true;
    [SerializeField] private bool showFacingIndicator = true;
    [SerializeField] private bool showAttackLabels = true;
    [SerializeField] private bool showOnlyActiveAttack = false;
    [SerializeField] private int activeAttackIndex = 0;

    [Header("Gizmo Style")]
    [SerializeField] private bool showWireframe = true;
    [SerializeField] private bool showFilled = true;
    [SerializeField] private float labelOffset = 0.5f;

    [Header("Facing Indicator Style")]
    [SerializeField] private float arrowLength = 0.3f;
    [SerializeField] private float arrowHeadSize = 0.08f;
    [SerializeField] private Color facingIndicatorColor = Color.yellow;

    // Cached references
    private EntityCombatModify _entityCombat;
    private Entity _entity;

    private void Awake()
    {
        _entityCombat = GetComponent<EntityCombatModify>();
        _entity = GetComponent<Entity>();

        if (_entityCombat == null)
            Debug.LogError("AttackGizmoVisualizer requires EntityCombat component!");
        if (_entity == null)
            Debug.LogError("AttackGizmoVisualizer requires Entity component!");
    }

    private void OnDrawGizmos()
    {
        if (!showAttackGizmos || _entityCombat == null || _entity == null) return;
        if (_entityCombat.attackDataArray == null || _entityCombat.targetCheck == null) return;

        DrawAttackGizmos();
    }

    private void DrawAttackGizmos()
    {
        for (var i = 0; i < _entityCombat.attackDataArray.Length; i++)
        {
            var attackData = _entityCombat.attackDataArray[i];
            if (attackData == null || !attackData.showInGizmos) continue;

            // Skip if showing only active attack, and this isn't it
            if (showOnlyActiveAttack && i != activeAttackIndex) continue;

            DrawSingleAttackGizmo(attackData, i);
        }
    }

    private void DrawSingleAttackGizmo(AttackDataSO attackData, int index)
    {
        // Get facing-aware position and angle
        Vector3 attackPosition = GetAttackPosition(attackData);
        float attackAngle = GetAttackAngle(attackData);

        // Set gizmo color
        Color baseColor = attackData.gizmoColor != Color.clear
            ? attackData.gizmoColor
            : GetIndexBasedColor(index);

        DrawRectangleGizmo(attackPosition, attackData, attackAngle, baseColor);

        // Draw the facing indicator at the center of this attack
        if (showFacingIndicator)
            DrawFacingIndicatorAtPosition(attackPosition);

        // Draw attack label
        if (showAttackLabels)
            DrawAttackLabel(attackPosition, index);
    }

    private void DrawRectangleGizmo(Vector3 position, AttackDataSO attackData, float angle, Color baseColor)
    {
        // Save the original matrix
        Matrix4x4 oldMatrix = Gizmos.matrix;

        // Apply rotation and position
        Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(0, 0, angle), Vector3.one);

        // Draw wireframe
        if (showWireframe)
        {
            Gizmos.color = baseColor;
            Gizmos.DrawWireCube(Vector3.zero, attackData.size);
        }

        // Draw filled rectangle
        if (showFilled)
        {
            Color fillColor = baseColor;
            fillColor.a = attackData.gizmoAlpha;
            Gizmos.color = fillColor;
            Gizmos.DrawCube(Vector3.zero, attackData.size);
        }

        // Restore the original matrix
        Gizmos.matrix = oldMatrix;
    }

    private void DrawFacingIndicatorAtPosition(Vector3 position)
    {
        // Set the facing indicator color
        Gizmos.color = facingIndicatorColor;

        // Calculate arrow end position
        Vector3 arrowEnd = position + Vector3.right * (_entity.FacingDirection * arrowLength);

        // Draw the main arrow line
        Gizmos.DrawLine(position, arrowEnd);

        // Draw arrowhead
        Vector3 arrowHead1 = arrowEnd + Vector3.up * arrowHeadSize + Vector3.left * (_entity.FacingDirection * arrowHeadSize);
        Vector3 arrowHead2 = arrowEnd + Vector3.down * arrowHeadSize + Vector3.left * (_entity.FacingDirection * arrowHeadSize);
        Gizmos.DrawLine(arrowEnd, arrowHead1);
        Gizmos.DrawLine(arrowEnd, arrowHead2);

        // Optional: Draw a small circle at the center for better visibility
        Gizmos.color = new Color(facingIndicatorColor.r, facingIndicatorColor.g, facingIndicatorColor.b, 0.5f);
        Gizmos.DrawWireSphere(position, 0.05f);
    }

    private void DrawAttackLabel(Vector3 position, int index)
    {
#if UNITY_EDITOR
        var labelPos = position + Vector3.up * labelOffset;
        UnityEditor.Handles.Label(labelPos, $"Attack {index}");
#endif
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

        return _entityCombat.targetCheck.position + (Vector3)facingAwareOffset;
    }

    private float GetAttackAngle(AttackDataSO attackData)
    {
        if (attackData.flipAngleWithFacing && !_entity.FacingRight)
            return -attackData.angle;
        return attackData.angle;
    }

    private Color GetIndexBasedColor(int index) =>
        Color.HSVToRGB(index / (float)_entityCombat.attackDataArray.Length, 1f, 0.8f);

    // Public methods for external control
    public void ToggleGizmos() => showAttackGizmos = !showAttackGizmos;
    public void SetActiveAttack(int index) => activeAttackIndex = index;
    public void ToggleShowOnlyActive() => showOnlyActiveAttack = !showOnlyActiveAttack;
    public void ToggleFacingIndicator() => showFacingIndicator = !showFacingIndicator;
}
