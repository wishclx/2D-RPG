using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    public Player_SkillManager skillManager { get; private set; }
    public Player player { get; private set; }
    public DamageScaleData damageScaleData { get; private set; }

    [Header("General details")]
    [SerializeField] protected SkillType skillType; // 技能类型。
    [SerializeField] protected SkillUpgradeType upgradeType; // 当前技能升级档位。
    [SerializeField] protected float cooldown;//冷却时间
    private float lastTimeUsed;//上次使用时间

    protected virtual void Awake()
    {
        skillManager = GetComponentInParent<Player_SkillManager>();
        player = GetComponentInParent<Player>();
        lastTimeUsed = -cooldown;
        damageScaleData = new DamageScaleData();// 初始化 damageScaleData，确保它在未升级时也有默认值。
    }


    public virtual void TryUseSkill()
    {

    }

    public void SetSkillUpgrade(SkillDataSO skillData)
    {
        UpgraedData upgrade = skillData.upgradeData;
        upgradeType = upgrade.upgradeType;
        cooldown = upgrade.cooldown;
        damageScaleData = upgrade.damageScaleData;


        player.ui.inGameUI.GetSkillSlots(skillType).SetupSkillSlot(skillData);// 更新UI以反映新的技能升级。
        ResetCooldown();// 升级后重置冷却，允许玩家立即使用新升级的技能。
    }

    public void ResetSkillUpgrade()
    {
        upgradeType = SkillUpgradeType.None;// 重置为未解锁状态
        cooldown = 0f;// 清空冷却配置
        damageScaleData = new DamageScaleData();// 重置伤害缩放数据
        lastTimeUsed = Time.time;

        UI_SkillSlot slot = player.ui.inGameUI.GetSkillSlotWithoutActivate(skillType);
        if (slot != null)
            slot.ResetToDefaultVisual();//恢复默认图标与冷却显示

        // 互斥槽位重置后只保留一个：保留 TimeShard，隐藏 TimeEcho
        if (skillType == SkillType.TimeShard || skillType == SkillType.TimeEcho)
        {
            UI_SkillSlot shardSlot = player.ui.inGameUI.GetSkillSlotWithoutActivate(SkillType.TimeShard);
            UI_SkillSlot echoSlot = player.ui.inGameUI.GetSkillSlotWithoutActivate(SkillType.TimeEcho);

            if (shardSlot != null)
                shardSlot.gameObject.SetActive(true);

            if (echoSlot != null)
                echoSlot.gameObject.SetActive(false);
        }
    }

    public virtual bool CanUseSkill()
    {
        if (upgradeType == SkillUpgradeType.None)
        {
            return false;
        }

        if (Oncooldown())
        {
            Debug.Log("技能冷却中");
            return false;
        }

        return true;
    }

    protected bool Unlocked(SkillUpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;
    public SkillUpgradeType GetUpgradeType() => upgradeType;
    public SkillType GetSkillType() => skillType;

    protected bool Oncooldown() => Time.time < lastTimeUsed + cooldown;
    public void SetSkillOnCooldown()
    {
        player.ui.inGameUI.GetSkillSlots(skillType).StartCooldown(cooldown);// 更新UI以反映技能进入冷却状态。
        lastTimeUsed = Time.time;
    }
    public void ReduceCooldownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;// 通过减少剩余冷却来缩短技能冷却。
    public void ResetCooldown()
    {
        player.ui.inGameUI.GetSkillSlots(skillType).ResetCooldown();// 更新UI以反映冷却重置。
        lastTimeUsed = Time.time - cooldown;
    }
}



