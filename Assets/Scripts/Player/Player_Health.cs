using System.Collections;
using UnityEngine;

// 玩家血量，增加存档接口实现以保存/恢复玩家生命值
public class Player_Health : Entity_Health, ISaveable
{
    private Player player;

    // 从存档中临时缓存的生命百分比（用于在属性/装备初始化后恢复）
    private float cachedHealthPercent = -1f;
    private bool hasCachedHealth = false;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();//获取玩家组件
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))//按下K键来测试玩家死亡
        {
            Die();//调用死亡方法
        }
    }

    protected override void Die()
    {
        base.Die();

        // 打开死亡界面
        player.ui.OpenDeathScreenUI();
    }

    // ISaveable: 保存当前血量、最大血量及百分比
    public void SaveData(ref GameData data)
    {
        if (data == null) return;

        data.playerHealth = GetCurrentHealth();
        data.playerMaxHealth = GetComponent<Entity_Stats>() != null ? GetComponent<Entity_Stats>().GetMaxHealth() : -1f;
        data.playerHealthPercent = GetHealthPercent();
    }

    // ISaveable: 从存档读取并缓存百分比，延迟应用以等待装备/属性初始化完成
    public void LoadData(GameData data)
    {
        if (data == null) return;

        // 优先使用保存的百分比，如果没有，但有绝对值与保存的最大值，则用两者计算百分比
        if (data.playerHealthPercent >= 0f)
        {
            cachedHealthPercent = Mathf.Clamp01(data.playerHealthPercent);
            hasCachedHealth = true;
        }
        else if (data.playerHealth >= 0f && data.playerMaxHealth > 0f)
        {
            cachedHealthPercent = Mathf.Clamp01(data.playerHealth / data.playerMaxHealth);
            hasCachedHealth = true;
        }
        else
        {
            hasCachedHealth = false;
        }

        if (hasCachedHealth)
        {
            // 延迟应用：等待一帧或两帧以确保 Player_Stats / 装备 已初始化
            StartCoroutine(ApplySavedHealthCo());
        }
    }

    private IEnumerator ApplySavedHealthCo()
    {
        // 等待两帧，尽量保证属性/装备已被 Start() / 初始化逻辑应用
        yield return null;
        yield return null;

        if (hasCachedHealth && cachedHealthPercent >= 0f)
        {
            SetHealthToPercent(cachedHealthPercent);// 按百分比设置当前生命，避免被最大值覆盖引起错误
            hasCachedHealth = false;
            cachedHealthPercent = -1f;
        }
    }
}
