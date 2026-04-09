using UnityEngine;

public interface IDamageable
{
    public bool TakeDamge(float damage,float elementalDamage, ElementType element,Transform damageDealer);// 接口方法，接受伤害值和伤害来源的Transform参数
}
