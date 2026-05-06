using TMPro;
using UnityEngine;

public class UI_StatToolTip : UI_ToolTip
{
    private Player_Stats playerStats;
    private TextMeshProUGUI statToolTipText;

    override protected void Awake()
    {
        base.Awake();
        playerStats = FindFirstObjectByType<Player_Stats>();
        statToolTipText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ShowToolTip(bool show, RectTransform targetRect, StatType statType)
    {
        base.ShowToolTip(show, targetRect);
        statToolTipText.text = GetStatTextByType(statType);
    }

    public string GetStatTextByType(StatType type)
    {
        switch (type)
        {
            // 核心属性
            case StatType.Strength:
                return "每点力量提高 1 点物理伤害。"
                     + "\n每点力量提高 0.5% 暴击伤害。";
            case StatType.Agility:
                return "每点敏捷提高 0.3% 暴击率。"
                     + "\n每点敏捷提高 0.5% 闪避率。";
            case StatType.Intelligence:
                return "每点智力提高 0.5% 元素抗性。"
                     + "\n每点智力额外增加 1 点元素伤害。"
                     + "\n若三种元素伤害都为 0，则不会获得该加成。";
            case StatType.Vitality:
                return "每点体质提高 5 点最大生命值。"
                     + "\n每点体质提高 1 点护甲。";

            // 物理输出
            case StatType.Damage:
                return "决定你的攻击造成的物理伤害。";
            case StatType.CritChance:
                return "决定你的攻击触发暴击的概率。";
            case StatType.CritPower:
                return "提高暴击时造成的伤害。";
            case StatType.ArmorReduction:
                return "你的攻击可无视目标护甲的百分比。";
            case StatType.AttackSpeed:
                return "决定你的攻击速度。";

            // 防御属性
            case StatType.MaxHealth:
                return "决定你的总生命值上限。";
            case StatType.HealthRegen:
                return "每秒恢复的生命值数量。";
            case StatType.Armor:
                return "降低受到的物理伤害。"
                     + "\n护甲减伤上限为 85%。"
                     + "\n当前减伤为：" + playerStats.GetArmorMitigation(0) * 100 + "%。";
            case StatType.Evasion:
                return "完全闪避攻击的概率。"
                     + "\n上限为 85%。";

            // 元素伤害
            case StatType.IceDamage:
                return "决定你的攻击造成的冰霜伤害。";
            case StatType.FireDamage:
                return "决定你的攻击造成的火焰伤害。";
            case StatType.LightningDamage:
                return "决定你的攻击造成的雷电伤害。";
            case StatType.ElementalDamage:
                return "元素伤害由三种元素共同构成。"
                     + "\n数值最高的元素会附加对应状态效果并造成完整伤害。"
                     + "\n其余两种元素各按 50% 作为额外加成。";

            // 元素抗性
            case StatType.IceResistance:
                return "降低受到的冰霜伤害。";
            case StatType.FireResistance:
                return "降低受到的火焰伤害。";
            case StatType.LightningResistance:
                return "降低受到的雷电伤害。";

            default:
                return "该属性暂无提示说明。";
        }
    }
}
