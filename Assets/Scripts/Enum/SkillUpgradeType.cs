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
    Shard_TeleportHpRewind, // 与碎片交换位置后，生命值恢复为创建碎片时的状态

    //------ 投掷分支 ------
    SwordThrow,        // 可投掷剑，从远处对敌人造成伤害
    SwordThrow_Spin,   // 投掷出的剑会在原地旋转并持续伤害敌人（类似电锯）
    SwordThrow_Pierce, // 投掷剑可穿透 N 个目标
    SwordThrow_Bounce, // 投掷剑会在敌人之间弹射

    // ----- 分身 -----   
    TimeEcho, // 创建一个玩家分身。它可以承受来自敌人的伤害
    TimeEcho_SingleAttack, // 分身可以执行一次攻击
    TimeEcho_ChanceToDuplicate,// 分身可以执行 N 次攻击
    TimeEcho_ChanceToMultiply, // 分身在攻击时有概率再生成一个分身

    TimeEcho_HealWisp, // 当分身死亡时，会生成一个飞向玩家的精灵来治疗玩家
                       // 治疗量 = 分身死亡时所受伤害的一定百分比
    TimeEcho_CleanseWisp,// 精灵现在会移除玩家身上的负面效果
    TimeEcho_CooldownWisp,// 精灵会将所有技能冷却减少 N 秒

    // ----- 领域展开 -----
    Domain_SlowingDown, // 创造一个区域，使敌人减速 90–100%，而你可以自由移动并战斗。
    Domain_EchoSpam,    // 你将无法移动，但可以用 Time Echo（分身）技能持续攻击敌人。
    Domain_ShardSpam // 你将无法移动，但可以用 Time Shard（碎片）技能持续攻击敌人。
}