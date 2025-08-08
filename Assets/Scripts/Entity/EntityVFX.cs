// This script controls special visual effects for game entities (like enemies or players)
// It specifically handles the visual effect when an entity takes damage
using System.Collections;
using UnityEngine;

// This class handles visual effects for game entities (like enemies or players)
public class EntityVFX : MonoBehaviour
{
    // This component is responsible for displaying the entity's image/sprite
    private SpriteRenderer _spriteRenderer;
    
    // Section for damage visual effect settings
    [Header("On Damage VFX")]
    // The special material (visual appearance) to use when the entity takes damage
    [SerializeField] private Material onDamageMaterial;
    // How long the damage effect should last (in seconds)
    [SerializeField] private float onDamageDuration = .1f;
    // Store the entity's original material so we can switch back to it after the effect
    private Material _originalMaterial;
    // Keeps track of the currently running damage effect so we can stop it if needed
    private Coroutine _onDamageCoroutineVFX;
    
    // This function runs when the entity is first created in the game
    private void Awake()
    {
        // Find the component that displays the entity's image
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // Save the entity's normal appearance so we can restore it later
        _originalMaterial = _spriteRenderer.material;
    }
    
    // This function is called when the entity takes damage to show a visual effect
    public void PlayOnDamageVFX()
    {
        // If there's already a damage effect running, stop it first
        if (_onDamageCoroutineVFX != null)
            StopCoroutine(_onDamageCoroutineVFX);
        
        // Start the damage visual effect
        _onDamageCoroutineVFX = StartCoroutine(OnDamageVFX());
    }

    // This is the actual damage visual effect sequence
    private IEnumerator OnDamageVFX()
    {
        // Change the entity's appearance to the damage effect
        _spriteRenderer.material = onDamageMaterial;
        // Wait for the specified duration (0.1 seconds by default)
        yield return new WaitForSeconds(onDamageDuration);
        // Change the entity's appearance back to normal
        _spriteRenderer.material = _originalMaterial;
    }
}
