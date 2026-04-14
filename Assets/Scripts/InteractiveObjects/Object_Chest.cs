using UnityEngine;

/// <summary>
/// Object_Chest 的职责说明。
/// </summary>
public class Object_Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX fx => GetComponent<Entity_VFX>();

    [Header("Open Deatils")]
    [SerializeField] private Vector2 knockback;

    /// <summary>
    /// 执行 TakeDamge 逻辑。
    /// </summary>
    public bool TakeDamge(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        fx.PlayOnDamageVfx();
        anim.SetBool("chestOpen", true);
        rb.linearVelocity = knockback;
        rb.angularVelocity = Random.Range(-200f, 200f);

        return true;
    }

}


