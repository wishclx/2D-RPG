using System;
using UnityEngine;

[Serializable]
public class Stat_OffenseGroup
{
    public Stat attackSpeed;

    // 物理伤害
    public Stat damage;
    public Stat critPower;//暴击伤害倍率
    public Stat critChance;//暴击率
    public Stat armorReduction;//护甲穿透

    // 元素伤害
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;
}
