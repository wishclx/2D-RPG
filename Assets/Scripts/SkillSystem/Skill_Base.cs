using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    [Header("General details")]
    [SerializeField] protected SkillType skillType;//技能类型
    [SerializeField] protected SkillUpgradeType upgradeType;//升级类型
    [SerializeField] private float cooldown;//冷却时间
    private float lastTimeUsed;//上次使用时间

    protected virtual void Awake()
    {
        lastTimeUsed = -cooldown;//初始化为负的冷却时间，使技能在游戏开始时可用
    }

    public void SetSkillUpgrade(UpgraedData upgrade)
    {
        upgradeType = upgrade.upgradeType;
        cooldown = upgrade.cooldown;
        //根据升级类型调整技能属性，例如增加伤害、减少冷却时间等
    }

    public bool CanUseSkill()
    {
        if (Oncooldown())
        {
            Debug.Log("技能cd中");
            return false;//如果在冷却中，不能使用技能
        }

        //检查其他条件，例如资源消耗、状态效果等

        return true;
    }

    protected bool Unlocked(SkillUpgradeType upgradeToCheck) => upgradeType >= upgradeToCheck;//检查技能是否解锁

    private bool Oncooldown() => Time.time < lastTimeUsed + cooldown;//是否在冷却中
    public void SetSkillOnCooldown() => lastTimeUsed = Time.time;//设置技能进入冷却
    public void ReserCooldownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;//通过减少冷却时间来重置技能的冷却
    public void ResetCooldown() => lastTimeUsed = Time.time;//重置技能的冷却
}
