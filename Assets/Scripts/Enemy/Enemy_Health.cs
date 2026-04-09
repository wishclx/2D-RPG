using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy => GetComponent<Enemy>();// 获取Enemy组件

    public override bool TakeDamge(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        bool wasHit = base.TakeDamge(damage, elementalDamage, element, damageDealer);

        if (wasHit == false)
            return false;

        if (damageDealer.GetComponent<Player>() != null)// 如果伤害来源是玩家，尝试进入战斗状态
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }
}
