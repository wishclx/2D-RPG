using UnityEngine;

/// <summary>
/// Skill_Base 的职责说明。
/// </summary>
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

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
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

    /// <summary>
    /// 执行 SetSkillUpgrade 逻辑。
    /// </summary>
    public void SetSkillUpgrade(UpgraedData upgrade)
    {
        upgradeType = upgrade.upgradeType;
        cooldown = upgrade.cooldown;
        damageScaleData = upgrade.damageScaleData;
    }

    /// <summary>
    /// 执行 CanUseSkill 逻辑。
    /// </summary>
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

    protected bool Oncooldown() => Time.time < lastTimeUsed + cooldown;
    public void SetSkillOnCooldown() => lastTimeUsed = Time.time;
    public void ReserCooldownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;// 通过减少剩余冷却来缩短技能冷却。
    public void ResetCooldown() => lastTimeUsed = Time.time;
}



