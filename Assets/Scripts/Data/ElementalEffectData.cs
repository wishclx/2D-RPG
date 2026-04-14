using UnityEngine;

public class ElementalEffectData
{
    public float chillDuration;// 冰冻持续时间。
    public float chillSlowMultiplier;

    public float burnDuration;// 燃烧持续时间。
    public float totalBurnDamage;

    public float shockDuration;// 感电持续时间。
    public float shockDamage;
    public float shockCharge;

    public ElementalEffectData(Entity_Stats entityStats, DamageScaleData damageScale)// 构造函数，根据实体属性和伤害缩放数据计算元素效果数据。
    {
        chillDuration = damageScale.chillDuration;
        chillSlowMultiplier = damageScale.chillSlowMultiplier;

        burnDuration = damageScale.burnDuration;
        totalBurnDamage = entityStats.offense.fireDamage.GetValue() * damageScale.burnDamageScale;

        shockDuration = damageScale.shockDuration;
        shockDamage = entityStats.offense.lightningDamage.GetValue() * damageScale.shockDamageScale;
        shockCharge = damageScale.shockCharge;
    }
}