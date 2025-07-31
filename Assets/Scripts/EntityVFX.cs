using System.Collections;
using UnityEngine;

public class EntityVFX : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    
    [Header("On Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageDuration = .1f;
    private Material _originalMaterial;
    private Coroutine _onDamageCoroutineVFX;
    
    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _originalMaterial = _spriteRenderer.material;
    }
    
    public void PlayOnDamageVFX()
    {
        if (_onDamageCoroutineVFX != null)
            StopCoroutine(_onDamageCoroutineVFX);
        
        _onDamageCoroutineVFX = StartCoroutine(OnDamageVFX());
    }

    private IEnumerator OnDamageVFX()
    {
        _spriteRenderer.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageDuration);
        _spriteRenderer.material = _originalMaterial;
    }
}
