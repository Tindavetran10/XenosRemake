using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Combat/Attack Data")]
public class AttackDataSO : ScriptableObject
{
    [Header("Rectangle Collision Detection")]
    public Vector2 size = new Vector2(2f, 1f);     // Width and Height of rectangle
    public Vector2 offset = Vector2.zero;          // Offset from the targetCheck position
    public float angle = 0f;                       // Rotation angle of rectangle
    
    [Header("Facing Direction Settings")]
    [Tooltip("If true, angle will be flipped when facing left")]
    public bool flipAngleWithFacing = true;
    [Tooltip("If true, offset will automatically be in front of character")]
    public bool offsetInFrontOfCharacter = true;
    
    [Header("Debug")]
    public Color gizmoColor = Color.red;
}