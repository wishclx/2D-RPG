using UnityEngine;

public class Skill_Dash : Skill_Base
{


    public void OnstartEffect()
    {
        // 起始阶段：根据已解锁升级效果生成分身/碎片。
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStart) || Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreatClone();

        if (Unlocked(SkillUpgradeType.Dash_ShardOnStart) || Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreatShard();
    }

    public void OnEndEffect()
    {
        // 结束阶段：根据已解锁升级效果生成分身/碎片。
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreatClone();

        if (Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreatShard();
    }

    private void CreatShard()
    {
        skillManager.shard.CreateRawShard();
    }

    private void CreatClone()
    {
        skillManager.timeEcho.CreateTimeEcho();
    }
}
