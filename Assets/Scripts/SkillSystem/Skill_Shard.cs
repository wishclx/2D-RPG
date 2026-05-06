using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Skill_Sharp 的职责说明。
/// </summary>
public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    private Entity_Health playerHealth;

    [SerializeField] private GameObject shardPrefab;//一个预制体，代表技能生成的碎片对象
    [SerializeField] private float detonateTime = 2f;//碎片生成后多久会爆炸

    [Header("Moving Shard Upgrade")]
    [SerializeField] private float shardSpeed = 7f;// 碎片向目标移动的速度

    [Header("Multicast Shard Upgrade")]
    [SerializeField] private int maxCharges = 3;// 最大碎片数量
    [SerializeField] private int currentCharges;
    [SerializeField] private bool isReCharging;

    [Header("Teleport Shard Upgrade")]
    [SerializeField] private float shardExistDuration = 10f;// 碎片存在的总时间

    [Header("Health Rewind Shard Upgrade")]
    [SerializeField] private float savedHealthPercent; // 在碎片生成时保存玩家当前生命值的百分比，在碎片爆炸时根据这个百分比恢复玩家生命值。

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;// 初始化当前碎片数量为最大值。
        playerHealth = GetComponentInParent<Entity_Health>();
    }
    public void CreateShard()
    {
        float detonateTime = GetDetonateTime();// 获取当前碎片的爆炸时间，根据是否解锁传送升级来决定。

        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();
        currentShard.SetupShard(this);

        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
            currentShard.OnExplode += ForceCooldown;// 如果解锁了传送升级，在碎片爆炸时强制技能进入冷却状态。
    }

    public void CreateRawShard(Transform target = null, bool shardsCanMove = false)
    {
        bool canMove = shardsCanMove != false ? shardsCanMove :
            Unlocked(SkillUpgradeType.Shard_MoveToEnemy) || Unlocked(SkillUpgradeType.Shard_MulticCast);

        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        // 直接创建一个碎片对象，并根据当前的升级状态设置它的属性。
        shard.GetComponent<SkillObject_Shard>().SetupShard(this, detonateTime, canMove, shardSpeed, target);
    }

    public void CreateDomainShard(Transform target)
    {

    }

    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)
            return;

        if (Unlocked(SkillUpgradeType.Shard))
            HandleShardRegular();

        if (Unlocked(SkillUpgradeType.Shard_MoveToEnemy))
            HandleShardMoving();

        if (Unlocked(SkillUpgradeType.Shard_MulticCast))
            HandleShardMulticast();

        if (Unlocked(SkillUpgradeType.Shard_Teleport))
            HandleShardTeleport();

        if (Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
            HandleShardHealthRewind();
    }

    private void HandleShardHealthRewind()
    {
        if (currentShard == null)
        {
            CreateShard();// 如果当前没有碎片，则创建一个新的碎片。
            savedHealthPercent = playerHealth.GetHealthPercent();
        }
        else
        {
            SwapPlayerAndShard();// 如果已经有一个碎片存在，则交换玩家和碎片的位置。
            playerHealth.SetHealthToPercent(savedHealthPercent);// 将玩家的生命值设置为之前保存的百分比，实现生命值回退的效果。
            SetSkillOnCooldown();// 设置技能进入冷却状态，防止立即再次使用。
        }
    }

    private void HandleShardTeleport()
    {
        if (currentShard == null)
        {
            CreateShard();// 如果当前没有碎片，则创建一个新的碎片。
        }
        else
        {
            SwapPlayerAndShard();// 如果已经有一个碎片存在，则交换玩家和碎片的位置。
        }
    }

    private void SwapPlayerAndShard()
    {
        Vector3 shardPosition = currentShard.transform.position;// 获取当前碎片的位置
        Vector3 playerPosition = player.transform.position;// 获取玩家当前位置

        currentShard.transform.position = playerPosition;
        currentShard.Explode();// 让碎片立即爆炸，造成伤害并播放效果。

        player.TeleportPlayer(shardPosition);
    }

    private void HandleShardMulticast()
    {
        if (currentCharges <= 0)
            return;

        CreateShard();
        currentShard.MoveToClosestTarget(shardSpeed);
        currentCharges--;

        if (isReCharging == false)
            StartCoroutine(ShardRechargeCo());
    }

    private IEnumerator ShardRechargeCo()
    {
        isReCharging = true;
        UI_SkillSlot slot = player.ui.inGameUI.GetSkillSlots(skillType);

        while (currentCharges < maxCharges)
        {
            if (slot != null)
                slot.StartCooldown(cooldown);// 每次恢复1层前，显示一次充能冷却

            yield return new WaitForSeconds(cooldown);
            currentCharges++;
        }

        isReCharging = false;
    }

    private void HandleShardMoving()
    {
        CreateShard();
        currentShard.MoveToClosestTarget(shardSpeed);

        SetSkillOnCooldown();// 设置技能进入冷却状态
    }

    private void HandleShardRegular()
    {
        CreateShard();
        SetSkillOnCooldown();// 设置技能进入冷却状态
    }

    public float GetDetonateTime()
    {
        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
            return shardExistDuration;// 如果解锁了传送碎片升级，返回碎片存在的总时间。

        return detonateTime;// 否则返回默认的爆炸时间。
    }

    private void ForceCooldown()
    {
        if (Oncooldown() == false)
        {
            SetSkillOnCooldown();// 如果技能不在冷却中，则强制设置技能进入冷却状态。
            currentShard.OnExplode -= ForceCooldown;// 取消订阅事件，避免重复调用。
        }
    }
}


