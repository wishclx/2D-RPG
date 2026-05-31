using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;
    private Player_QuestManager questManager;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
        // 使用安全访问，避免 Player.instance 为 null 时直接抛异常
        questManager = Player.instance != null ? Player.instance.questManager : null;
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

        // 给玩家加金币：先检查引用，避免 NullReferenceException
        if (enemy != null && enemy.GoldDrop > 0)
        {
            if (Player.instance != null && Player.instance.inventory != null)
            {
                Player.instance.inventory.AddGold(enemy.GoldDrop);//敌人死亡时直接把金币加到玩家
            }
            else
            {
                Debug.LogWarning("Enemy_Health.Die: 无法给玩家添加金币，Player.instance 或 inventory 为 null");
            }
        }

        // 添加任务进度：先检查 questManager 和 questTargetId
        if (questManager != null)
        {
            if (enemy != null && string.IsNullOrEmpty(enemy.questTargetId) == false)
            {
                questManager.AddProgress(enemy.questTargetId);
            }
            else
            {
                Debug.LogWarning("Enemy_Health.Die: 无效的 questTargetId，无法添加任务进度");
            }
        }
        else
        {
            Debug.LogWarning("Enemy_Health.Die: questManager 为 null，无法添加任务进度");
        }
    }
}


