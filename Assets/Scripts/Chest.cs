using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D Rb => GetComponent<Rigidbody2D>();
    private Animator Anim => GetComponentInChildren<Animator>();
    private EntityVFX VFX => GetComponent<EntityVFX>();
        
    [Header("Open Chest Details")]
    [SerializeField] private Vector2 knockback;
    public void TakeDamage(float damage, Transform damageDealer = null)
    {
        Debug.Log("Chest is opened");
        
        VFX.PlayOnDamageVFX();
        //Anim.SetBool("openChest", true);
        Rb.linearVelocity = knockback;
        
        Rb.angularVelocity = Random.Range(-200f, 200f);
        
        //Drop Item
    }
}
