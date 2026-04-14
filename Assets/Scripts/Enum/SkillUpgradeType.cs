using UnityEngine;

public enum SkillUpgradeType
{
    None,

    // ------ 冲刺分支 ------
    Dash, // 冲刺以规避伤害
    Dash_CloneOnStart, // 冲刺开始时生成一个分身
    Dash_CloneOnStartAndArrival, // 冲刺开始和结束时各生成一个分身
    Dash_ShardOnStart, // 冲刺开始时生成一个碎片
    Dash_ShardOnStartAndArrival, // 冲刺开始和结束时各生成一个碎片

    // ------ 碎片分支 ------
    Shard, // 碎片被敌人触碰或持续时间结束时会爆炸
    Shard_MoveToEnemy, // 碎片会移动到最近的敌人
    Shard_MulticCast, // 碎片技能最多可拥有 N 层充能，可连续释放
    Shard_Teleport, // 可与最近创建的碎片交换位置
    Shard_TeleportHpRewind // 与碎片交换位置后，生命值恢复为创建碎片时的状态
}