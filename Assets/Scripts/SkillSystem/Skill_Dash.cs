using UnityEngine;

/// <summary>
/// Skill_Dash 的职责说明。
/// </summary>
public class Skill_Dash : Skill_Base
{


    /// <summary>
    /// 执行 OnstartEffect 逻辑。
    /// </summary>
    public void OnstartEffect()
    {
        // 起始阶段：根据已解锁升级效果生成分身/碎片。
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStart) || Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreatClone();

        if (Unlocked(SkillUpgradeType.Dash_ShardOnStart) || Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreatShard();
    }

    /// <summary>
    /// 执行 OnEndEffect 逻辑。
    /// </summary>
    public void OnEndEffect()
    {
        // 结束阶段：根据已解锁升级效果生成分身/碎片。
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreatClone();

        if (Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreatShard();
    }

    /// <summary>
    /// 执行 CreatShaed 逻辑。
    /// </summary>
    private void CreatShard()
    {
        skillManager.shard.CreateRawShard();
    }

    /// <summary>
    /// 执行 CreatClone 逻辑。
    /// </summary>
    private void CreatClone()
    {
        Debug.Log("创建了一个分身");
    }
}



