using System;
using UnityEngine;

[Serializable]
/// <summary>
/// Stat_OffenseGroup 的职责说明。
/// </summary>
public class Stat_OffenseGroup
{
    public Stat attackSpeed;

    //物理伤害
    public Stat damage;
    public Stat critPower;//暴击伤害
    public Stat critChance;//暴击率
    public Stat armorReduction;

    // 元素伤害
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;
}


