using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;
    private Player_QuestManager questManager;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
        questManager = Player.instance.questManager;
    }

    public override bool TakeDamge(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (canTakeDamage == false)//如果不能受到伤害，直接返回 false。
            return false;

        bool wasHit = base.TakeDamge(damage, elementalDamage, element, damageDealer);

        if (wasHit == false)
            return false;

        if (damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }

    protected override void Die()
    {
        base.Die();

        if (enemy != null && enemy.GoldDrop > 0)
            Player.instance.inventory.AddGold(enemy.GoldDrop);//敌人死亡时直接把金币加到玩家

        questManager.AddProgress(enemy.questTargetId);
    }
}


