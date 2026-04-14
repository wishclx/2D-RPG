using UnityEngine;

// 负责实体属性相关计算（伤害、抗性、闪避、护甲减伤等）。
/// <summary>
/// 实体属性系统入口。
/// 提供物理/元素伤害、抗性、减伤、闪避、最大生命值等计算能力。
/// </summary>
public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaultStatSetup;// 默认属性配置，用于一键回填基础值。

    public Stat_ResourceGroup resources;// 资源类属性（生命、回复等）。
    public Stat_OffenseGroup offense;// 进攻类属性（伤害、暴击等）。
    public Stat_DefenseGroup defense;// 防御类属性（护甲、抗性、闪避等）。
    public Stat_MajorGroup major;// 主属性（力量、敏捷、智力、体力）。

    public AttackData GetAttackData(DamageScaleData scaleData)
    {
        return new AttackData(this, scaleData);// 通过当前属性和外部伤害倍率数据，构造完整的攻击数据对象。
    }

    /// <summary>
    /// 执行 GetElementalDamage 逻辑。
    /// </summary>
    public float GetElementalDamage(out ElementType element, float scaleFactor = 1)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightingDamage = offense.lightningDamage.GetValue();
        float bonusElementalDamage = major.intelligence.GetValue() * 1f; // 智力提供的额外元素伤害。

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
        // 选出基础伤害最高的元素，作为主元素。

        if (highestDamage <= 0)
        {
            element = ElementType.None; // 没有有效元素伤害时，返回 None。
            return 0; // 无元素伤害，直接返回 0。
        }

        float bonusFire = (fireDamage == highestDamage) ? 0 : fireDamage * .5f;// 非主元素按 50% 计入混合加成。
        float bonusIce = (iceDamage == highestDamage) ? 0 : iceDamage * .5f;
        float bonusLighting = (lightingDamage == highestDamage) ? 0 : lightingDamage * .5f;

        float weakerElementalDamage = bonusFire + bonusIce + bonusLighting; // 次元素的合计加成。
        float finalDamage = highestDamage + weakerElementalDamage + bonusElementalDamage; // 最终元素伤害。

        return finalDamage * scaleFactor; // 应用外部倍率（如技能倍率）。
    }

    /// <summary>
    /// 执行 GetElementalResistance 逻辑。
    /// </summary>
    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * .5f; // 智力提供的额外元素抗性。

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
                return 0; // 非元素类型不提供元素抗性。
        }

        float resitance = baseResistance + bonusResistance;
        float resistanceCap = 85f; // 抗性上限（85%）。
        float finalResistance = Mathf.Clamp(resitance, 0, resistanceCap) / 100; // 转为 0~1 的减伤比例。

        return finalResistance;
    }

    /// <summary>
    /// 执行 GetPhyiscalDamage 逻辑。
    /// 计算物理伤害，并输出本次是否触发暴击。
    /// </summary>
    public float GetPhyiscalDamage(out bool isCrit, float scaleFactor = 1)// 计算物理伤害并输出是否暴击。
    {
        float baseDamage = offense.damage.GetValue();// 基础物理伤害。
        float bonusDamage = major.strength.GetValue();// 力量提供的额外物理伤害。
        float totalBaseDamage = baseDamage + bonusDamage;// 暴击结算前的总基础伤害。

        float baseCritChance = offense.critChance.GetValue();// 基础暴击率。
        float bonusCritChance = major.agility.GetValue() * .3f;// 敏捷提供的暴击率加成。
        float critChance = baseCritChance + bonusCritChance;// 最终暴击率。

        float baseCripower = offense.critPower.GetValue();
        float bonusCritPower = major.strength.GetValue() * .5f;// 力量提供的暴击伤害加成。
        float critPower = (baseCripower + bonusCritPower) / 100;// 百分比转换为倍率。

        isCrit = Random.Range(0, 100) < critChance;// 随机判定是否触发暴击。
        float finalDamage = isCrit ? totalBaseDamage * critPower : totalBaseDamage;// 暴击时按暴伤倍率结算。

        return finalDamage * scaleFactor;
    }

    /// <summary>
    /// 执行 GetArmorMitigation 逻辑。
    /// </summary>
    public float GetArmorMitigation(float armorReduction)
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue() * 1f; // 体力提供的护甲加成。
        float totalArmor = baseArmor + bonusArmor;

        float reductionMuliplier = Mathf.Clamp(1 - armorReduction, 0, 1); // 破甲后剩余的护甲比例。
        float effectiveArmor = totalArmor * reductionMuliplier; // 实际生效护甲。

        float mitigation = effectiveArmor / (effectiveArmor + 100); // 护甲减伤公式。
        float mitigationCap = 0.85f; // 护甲减伤上限 85%。

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap); // 限制到有效区间。

        return finalMitigation;
    }
    /// <summary>
    /// 执行 GetArmorReduction 逻辑。
    /// </summary>
    public float GetArmorReduction()
    {
        float finalReduction = offense.armorReduction.GetValue() / 100; // 破甲值由百分比转换为倍率。

        return finalReduction;
    }

    /// <summary>
    /// 执行 GetEvasion 逻辑。
    /// </summary>
    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 0.5f; // 敏捷提供的额外闪避值。

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 85; // 闪避上限（85%）。

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap); // 将闪避限制在有效范围。

        return finalEvasion;
    }
    /// <summary>
    /// 执行 GetMaxHealth 逻辑。
    /// </summary>
    public float GetMaxHealth()
    {
        float baseMaxHealth = resources.maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5; // 体力提供的生命值加成。

        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;
        return finalMaxHealth;
    }

    /// <summary>
    /// 执行 GetStatByType 逻辑。
    /// </summary>
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


    [ContextMenu("Update Default Stat Setup")]// 右键菜单：将默认配置写入当前属性。
    /// <summary>
    /// 执行 ApplyDefaultStatSetup 逻辑。
    /// </summary>
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
