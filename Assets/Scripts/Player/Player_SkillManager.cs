using UnityEngine;

/// <summary>
/// Player_SkillManager 的职责说明。
/// </summary>
public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_SwordThrow swordThrow { get; private set; }

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();
    }

    /// <summary>
    /// 执行 GetSkillByType 逻辑。
    /// </summary>
    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Dash:
                return dash;

            case SkillType.TimeShard:
                return shard;

            default:
                Debug.Log($"Skill type {type} not found.");
                return null;
        }
    }
}
