using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Player_Stats playerStats;
    private RectTransform rect;
    private UI ui;

    [SerializeField] private StatType statSlotType;
    [SerializeField] private TextMeshProUGUI statName;
    [SerializeField] private TextMeshProUGUI statValue;

    private void OnValidate()
    {
        gameObject.name = "属性 - " + GetStatNameByType(statSlotType);
        statName.text = GetStatNameByType(statSlotType);
    }

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        playerStats = FindFirstObjectByType<Player_Stats>();//保证没有敌人对象,否则会拿到敌人的属性
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.ShowToolTip(true, rect, statSlotType);//显示属性提示框，传入属性类型以便提示框显示对应的文本
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.ShowToolTip(false, null);//隐藏属性提示框
    }

    public void UpdateStatValue()
    {
        Stat statToUpdate = playerStats.GetStatByType(statSlotType);//根据类型获取对应的属性

        if (statToUpdate == null && statSlotType != StatType.ElementalDamage)//元素伤害是一个特殊属性，不直接对应一个Stat对象，而是通过计算得到的
        {
            Debug.LogWarning("属性 " + statSlotType + " 不存在于玩家属性组件中！");
            return;
        }

        float value = 0;//默认值为0，如果属性不存在则显示0

        switch (statSlotType)
        {
            //主要属性
            case StatType.Strength:
                value = playerStats.major.strength.GetValue();
                break;
            case StatType.Agility:
                value = playerStats.major.agility.GetValue();
                break;
            case StatType.Intelligence:
                value = playerStats.major.intelligence.GetValue();
                break;
            case StatType.Vitality:
                value = playerStats.major.vitality.GetValue();
                break;

            //进攻属性
            case StatType.Damage:
                value = playerStats.GetBaseDamage();
                break;
            case StatType.CritChance:
                value = playerStats.GetCritChance();
                break;
            case StatType.CritPower:
                value = playerStats.GetCritPower();
                break;
            case StatType.ArmorReduction:
                value = playerStats.GetArmorReduction() * 100;
                break;
            case StatType.AttackSpeed:
                value = playerStats.offense.attackSpeed.GetValue() * 100;
                break;

            //防御属性
            case StatType.MaxHealth:
                value = playerStats.GetMaxHealth();
                break;
            case StatType.HealthRegen:
                value = playerStats.resources.healthRegen.GetValue();
                break;
            case StatType.Evasion:
                value = playerStats.GetEvasion();
                break;
            case StatType.Armor:
                value = playerStats.GetBaseArmor();
                break;

            //元素伤害属性
            case StatType.IceDamage:
                value = playerStats.offense.iceDamage.GetValue();
                break;
            case StatType.FireDamage:
                value = playerStats.offense.fireDamage.GetValue();
                break;
            case StatType.LightningDamage:
                value = playerStats.offense.lightningDamage.GetValue();
                break;
            case StatType.ElementalDamage:
                value = playerStats.GetElementalDamage(out ElementType element, 1);
                break;

            //元素抗性属性
            case StatType.IceResistance:
                value = playerStats.GetElementalResistance(ElementType.Ice) * 100;
                break;
            case StatType.FireResistance:
                value = playerStats.GetElementalResistance(ElementType.Fire) * 100;
                break;
            case StatType.LightningResistance:
                value = playerStats.GetElementalResistance(ElementType.Lightning) * 100;
                break;
        }

        statValue.text = IsPercentageStat(statSlotType) ? value + "%" : value.ToString();
    }

    private bool IsPercentageStat(StatType type)
    {
        switch (type)
        {
            case StatType.CritChance:
            case StatType.CritPower:
            case StatType.ArmorReduction:
            case StatType.IceResistance:
            case StatType.FireResistance:
            case StatType.LightningResistance:
            case StatType.AttackSpeed:
            case StatType.Evasion:
                return true;
            default:
                return false;
        }
    }

    private string GetStatNameByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return "最大生命";
            case StatType.HealthRegen: return "生命回复";
            case StatType.Armor: return "护甲";
            case StatType.Evasion: return "闪避";

            case StatType.Strength: return "力量";
            case StatType.Agility: return "敏捷";
            case StatType.Intelligence: return "智力";
            case StatType.Vitality: return "活力";

            case StatType.AttackSpeed: return "攻击速度";
            case StatType.Damage: return "攻击";
            case StatType.CritChance: return "暴击率";
            case StatType.CritPower: return "暴击伤害";
            case StatType.ArmorReduction: return "破甲";

            case StatType.FireDamage: return "火焰伤害";
            case StatType.IceDamage: return "冰霜伤害";
            case StatType.LightningDamage: return "雷电伤害";
            case StatType.ElementalDamage: return "元素伤害";

            case StatType.IceResistance: return "冰抗";
            case StatType.FireResistance: return "火抗";
            case StatType.LightningResistance: return "雷抗";

            default: return "未知属性";
        }
    }

}
