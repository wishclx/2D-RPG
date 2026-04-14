using UnityEngine;

public interface IDamageable
{
    public bool TakeDamge(float damage,float elementalDamage, ElementType element,Transform damageDealer);
}


