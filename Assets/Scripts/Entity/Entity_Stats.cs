using UnityEngine;

// 统计类，记录实体的各种统计数据，如伤害、治疗、击杀数等
public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaultStatSetup;//默认的统计数据设置，可以在Unity编辑器中配置

    public Stat_ResourceGroup resources;//资源属性
    public Stat_OffenseGroup offense;//攻击属性
    public Stat_DefenseGroup defense;//防御属性
    public Stat_MajorGroup major;//主要属性

    public float GetElementalDamage(out ElementType element, float scaleFactor = 1)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightingDamage = offense.lightningDamage.GetValue();
        float bonusElementalDamage = major.intelligence.GetValue() * 1f; // 每点智力增加1点元素伤害

        float highestDamage = fireDamage;
        element = ElementType.Fire;

        if (iceDamage > highestDamage)
        {
            highestDamage = iceDamage;
            element = ElementType.Ice;
        }

        if (lightingDamage > highestDamage)
        {
            highestDamage = lightingDamage;
            element = ElementType.Lightning;
        }
        //float highestDamage = Mathf.Max(fireDamage, iceDamage, lightingDamage);//选择最高的元素伤害作为基础元素伤害

        if (highestDamage <= 0)
        {
            element = ElementType.None; // 如果没有元素伤害，元素类型设为None
            return 0; // 如果没有元素伤害，直接返回0
        }

        float bonusFire = (fireDamage == highestDamage) ? 0 : fireDamage * .5f;//如果火焰伤害不是最高的元素伤害，则给予50%的火焰伤害加成
        float bonusIce = (iceDamage == highestDamage) ? 0 : iceDamage * .5f;
        float bonusLighting = (lightingDamage == highestDamage) ? 0 : lightingDamage * .5f;

        float weakerElementalDamage = bonusFire + bonusIce + bonusLighting; // 计算较弱元素伤害的加成
        float finalDamage = highestDamage + weakerElementalDamage + bonusElementalDamage; // 计算总元素伤害

        return finalDamage * scaleFactor; // 根据传入的缩放因子调整最终元素伤害值
    }

    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * .5f; // 每点智力增加0.5%元素抗性

        switch (element)
        {
            case ElementType.Fire:
                baseResistance = defense.fireRes.GetValue();
                break;
            case ElementType.Ice:
                baseResistance = defense.iceRes.GetValue();
                break;
            case ElementType.Lightning:
                baseResistance = defense.lightningRes.GetValue();
                break;
            default:
                return 0; // 如果元素类型为None或其他未定义类型，返回0抗性
        }

        float resitance = baseResistance + bonusResistance;
        float resistanceCap = 85f; // 元素抗性上限为85%
        float finalResistance = Mathf.Clamp(resitance, 0, resistanceCap) / 100; // 确保最终元素抗性不超过上限，并转换为小数形式

        return finalResistance;
    }

    public float GetPhyiscalDamage(out bool isCrit, float scaleFactor = 1)//计算物理伤害的方法，返回最终伤害值，并通过输出参数isCrit指示是否暴击
    {
        float baseDamage = offense.damage.GetValue();//基础伤害
        float bonusDamage = major.strength.GetValue();//每点力量增加1点伤害
        float totalBaseDamage = baseDamage + bonusDamage;//总伤害

        float baseCritChance = offense.critChance.GetValue();//基础暴击率    
        float bonusCritChance = major.agility.GetValue() * .3f;//每点敏捷增加0.3%暴击率
        float critChance = baseCritChance + bonusCritChance;// 总暴击率

        float baseCripower = offense.critPower.GetValue();
        float bonusCritPower = major.strength.GetValue() * .5f;//每点力量增加0.5倍暴击伤害倍率
        float critPower = (baseCripower + bonusCritPower) / 100;//总暴击伤害倍率

        isCrit = Random.Range(0, 100) < critChance;//是否暴击
        float finalDamage = isCrit ? totalBaseDamage * critPower : totalBaseDamage;//最终伤害

        return finalDamage * scaleFactor;
    }

    public float GetArmorMitigation(float armorReduction)
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue() * 1f; //每点体质增加1点护甲
        float totalArmor = baseArmor + bonusArmor;

        float reductionMuliplier = Mathf.Clamp(1 - armorReduction, 0, 1); //将护甲穿透率转换为减伤乘数，确保在0到1之间
        float effectiveArmor = totalArmor * reductionMuliplier; // 计算穿透后的有效护甲值

        float mitigation = effectiveArmor / (effectiveArmor + 100); //护甲减伤公式
        float mitigationCap = 0.85f; //减伤上限为85%

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap); //确保最终减伤不超过上限

        return finalMitigation;
    }
    public float GetArmorReduction()
    {
        float finalReduction = offense.armorReduction.GetValue() / 100; //护甲穿透率转换为小数形式

        return finalReduction;
    }

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 0.5f; // 每点敏捷增加0.5%闪避率

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 85; // 闪避率上限为85%

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap); // 确保最终闪避率不超过上限

        return finalEvasion;
    }
    public float GetMaxHealth()
    {
        float baseMaxHealth = resources.maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5; // 每点体质增加10点生命值

        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;
        return finalMaxHealth;
    }

    public Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return resources.maxHealth;
            case StatType.HealthRegen: return resources.healthRegen;

            case StatType.Strength: return major.strength;
            case StatType.Agility: return major.agility;
            case StatType.Intelligence: return major.intelligence;
            case StatType.Vitality: return major.vitality;

            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.Damage: return offense.damage;
            case StatType.CritChance: return offense.critChance;
            case StatType.CritPower: return offense.critPower;
            case StatType.ArmorReduction: return offense.armorReduction;

            case StatType.FireDamage: return offense.fireDamage;
            case StatType.IceDamage: return offense.iceDamage;
            case StatType.LightningDamage: return offense.lightningDamage;

            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;

            case StatType.IceResistance: return defense.iceRes;
            case StatType.FireResistance: return defense.fireRes;
            case StatType.LightningResistance: return defense.lightningRes;

            default:
                Debug.LogWarning($"StatType {type} not implemented yet.");
                return null;
        }
    }


    [ContextMenu("Update Default Stat Setup")]//这个标签允许我们在Unity编辑器中右键点击组件时，直接调用这个方法来更新默认统计数据设置
    public void ApplyDefaultStatSetup()
    {
        if (defaultStatSetup == null)
        {
            Debug.Log("No default stat setup assigned");
            return;
        }

        resources.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);

        major.strength.SetBaseValue(defaultStatSetup.strength);
        major.agility.SetBaseValue(defaultStatSetup.agility);
        major.intelligence.SetBaseValue(defaultStatSetup.intelligence);
        major.vitality.SetBaseValue(defaultStatSetup.vitality);

        offense.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        offense.damage.SetBaseValue(defaultStatSetup.damage);
        offense.critChance.SetBaseValue(defaultStatSetup.critChance);
        offense.critPower.SetBaseValue(defaultStatSetup.critPower);
        offense.armorReduction.SetBaseValue(defaultStatSetup.armorReduction);

        offense.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        offense.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        offense.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);

        defense.armor.SetBaseValue(defaultStatSetup.armor);
        defense.evasion.SetBaseValue(defaultStatSetup.evasion);

        defense.iceRes.SetBaseValue(defaultStatSetup.iceResistance);
        defense.fireRes.SetBaseValue(defaultStatSetup.fireResistance);
        defense.lightningRes.SetBaseValue(defaultStatSetup.lightningResistance);
    }
}
