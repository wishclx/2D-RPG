using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab;// 分身预制体
    [SerializeField] private float timeEchoDuration;

    [Header("Attack Upgrades")]
    [SerializeField] private int maxAttacks = 3;// 最大攻击次数
    [SerializeField] private float duplicateChance = .3f;// 分身攻击的概率

    [Header("Heal wisp Upgrades")]
    [SerializeField] private float damagePercentHealed = .3f;// 治疗分身升级的治疗百分比
    [SerializeField] private float cooldownReducedInSeconds;// 冷却分身升级的冷却时间减少量

    public float GetPercentOfDamageHealed()
    {
        if (ShouldBeWisp() == false)
            return 0;

        return damagePercentHealed;// 如果升级类型是治疗分身，返回治疗百分比，否则返回0
    }

    public float GetCooldownReduceInSeconds()
    {
        if (upgradeType != SkillUpgradeType.TimeEcho_CooldownWisp)
            return 0;

        return cooldownReducedInSeconds;// 如果升级类型是冷却分身，返回冷却时间减少量，否则返回0
    }

    public bool CanRemoveNegativeEffects()
    {
        return upgradeType == SkillUpgradeType.TimeEcho_CleanseWisp;// 如果升级类型是净化分身，返回 true，否则返回 false
    }

    public bool ShouldBeWisp()
    {
        return upgradeType == SkillUpgradeType.TimeEcho_HealWisp
            || upgradeType == SkillUpgradeType.TimeEcho_CleanseWisp
            || upgradeType == SkillUpgradeType.TimeEcho_CooldownWisp;// 如果升级类型是治疗、净化或冷却分身，返回 true，否则返回 false
    }

    public float GetDuplicateChance()
    {
        if (upgradeType != SkillUpgradeType.TimeEcho_ChanceToDuplicate)
            return 0;

        return duplicateChance;// 如果是有概率复制的升级，返回复制概率，否则返回0
    }

    public int GetMaxAttacks()
    {
        if (upgradeType == SkillUpgradeType.TimeEcho_SingleAttack || upgradeType == SkillUpgradeType.TimeEcho_ChanceToDuplicate)
            return 1;// 如果是单次攻击或有概率复制的升级，分身只能攻击一次

        if (upgradeType == SkillUpgradeType.TimeEcho_ChanceToMultiply)
            return maxAttacks;// 如果是有概率多次攻击的升级，分身可以攻击最大次数

        return 0;
    }

    public float GetEchoDuration()
    {
        return timeEchoDuration;
    }

    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)
            return;

        CreateTimeEcho();
        SetSkillOnCooldown();// 设置技能冷却时间
    }

    public void CreateTimeEcho(Vector3? targetPosition = null)// 创建分身的方法，接受一个可选的目标位置参数
    {
        // 使用 null 合并运算符，如果 targetPosition 不为 null，则使用其值，否则使用 transform.position 作为分身生成的位置。
        Vector3 position = targetPosition ?? transform.position;

        GameObject timeEcho = Instantiate(timeEchoPrefab, position, Quaternion.identity);// 在指定位置生成分身预制体
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this);
    }
}
