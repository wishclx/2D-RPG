using UnityEngine;

public enum SkillUpgradeType
{
    // ------- 冲刺技能树 -------
    Dash, //冲刺免疫伤害
    Dash_CloneOnStart,              // 冲刺开始时创建一个分身
    Dash_CloneOnStartAndArrival,    // 冲刺开始和到达时各创建一个分身
    Dash_ShardOnStart,              // 冲刺开始时创建一个碎片
    Dash_ShardOnStartAndArrival     // 冲刺开始和到达时各创建一个碎片
}
