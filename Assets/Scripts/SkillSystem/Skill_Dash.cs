using UnityEngine;

public class Skill_Dash : Skill_Base
{


    public void OnstartEffect()
    {
        // 当已解锁“冲刺开始生成分身”或“冲刺开始/到达均生成分身”任一升级时，
        // 在冲刺开始阶段触发分身创建逻辑。
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStart) || Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreatClone();//创造一个分身

        if (Unlocked(SkillUpgradeType.Dash_ShardOnStart) || Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreatShaed();//创造一个影子
    }

    public void OnEndEffect()
    {
        // 当已解锁“冲刺开始/到达均生成分身”升级时，
        // 在冲刺结束阶段触发分身创建逻辑。
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreatClone();//创造一个分身

        if (Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreatShaed();//创造一个影子
    }

    private void CreatShaed()
    {
        Debug.Log("创造了一个影子");

        //在这里可以添加创造影子分身的逻辑，例如实例化一个分身对象，设置其位置和行为等
    }

    private void CreatClone()
    {
        Debug.Log("创造了一个分身");
    }
}
