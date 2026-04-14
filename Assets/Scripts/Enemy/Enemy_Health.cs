using UnityEngine;

/// <summary>
/// Enemy_Health 的职责说明。
/// </summary>
public class Enemy_Health : Entity_Health
{
    private Enemy enemy => GetComponent<Enemy>();

    /// <summary>
    /// 执行 TakeDamge 逻辑。
    /// </summary>
    public override bool TakeDamge(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        bool wasHit = base.TakeDamge(damage, elementalDamage, element, damageDealer);

        if (wasHit == false)
            return false;

        if (damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }
}


