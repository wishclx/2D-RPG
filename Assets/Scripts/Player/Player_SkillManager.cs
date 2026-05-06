using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_SwordThrow swordThrow { get; private set; }
    public Skill_TimeEcho timeEcho { get; private set; }
    public Skill_DomainExpansion domainExpansion { get; private set; }

    public Skill_Base[] allSkills { get; private set; }// 存储所有技能对象的数组，方便管理和访问。

    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();
        timeEcho = GetComponentInChildren<Skill_TimeEcho>();
        domainExpansion = GetComponentInChildren<Skill_DomainExpansion>();

        allSkills = GetComponentsInChildren<Skill_Base>();
    }

    public void ReduceAllSkillCooldownBy(float amout)
    {
        foreach (var skill in allSkills)
            skill.ReduceCooldownBy(amout);
    }

    public void ResetAllSkillUpgrades()
    {
        foreach (var skill in allSkills)
            skill.ResetSkillUpgrade();// 统一重置所有技能升级类型
    }

    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Dash:
                return dash;

            case SkillType.TimeShard:
                return shard;

            case SkillType.SwordThrow:
                return swordThrow;

            case SkillType.TimeEcho:
                return timeEcho;

            case SkillType.DomainExpansion:
                return domainExpansion;

            default:
                Debug.Log($"Skill type {type} not found.");
                return null;
        }
    }
}
