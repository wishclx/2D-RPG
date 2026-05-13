using System;
using System.Text;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    private string itemId;

    public ItemDataSO itemData;//物品数据
    public int stackSize = 1;//物品堆叠数量

    public ItemModifier[] modifiers { get; private set; }//表示这个物品的属性加成值，可以有多个属性加成值，例如攻击力加成、防御力加成等
    public ItemEffect_DataSO itemEffect;//表示这个物品的特殊效果数据，例如使用后回复生命、增加攻击速度等

    public int buyPrice { get; private set; }
    public int sellPrice { get; private set; }

    public Inventory_Item(ItemDataSO itemData)
    {
        this.itemData = itemData;//构造函数，初始化物品数据
        itemEffect = itemData.itemEffect;//获取物品的效果数据
        buyPrice = itemData.itemPrice;//获取物品的购买价格
        sellPrice = (int)(itemData.itemPrice * .35f);//计算物品的出售价格，通常是购买价格的一半

        modifiers = EquipmentData()?.modifiers;//如果物品数据是装备数据，则获取装备数据中的属性加成值，并赋值给modifiers属性
        itemId = itemData.itemName + " - " + Guid.NewGuid();//生成一个唯一的物品ID，使用物品名称加上一个GUID（全局唯一标识符）来确保每个物品都有一个独特的ID
    }

    public void AddModifiers(Entity_Stats playerstats)
    {
        foreach (var mod in modifiers)
        {
            Stat statToModify = playerstats.GetStatByType(mod.statType);//获取玩家的对应属性
            statToModify.AddModifier(mod.value, itemId);//给玩家的属性添加加成值，来源是物品ID
        }
    }

    public void RemoveModifiers(Entity_Stats playerstats)
    {
        foreach (var mod in modifiers)
        {
            Stat statToModify = playerstats.GetStatByType(mod.statType);//获取玩家的对应属性
            statToModify.RemoveModifier(itemId);//从玩家的属性中移除加成值，来源是物品ID
        }
    }

    //如果物品有效果数据，则订阅玩家的事件，以便在玩家触发相关事件时，物品效果能够生效
    public void AddItemEffect(Player player) => itemEffect?.Subscribe(player);

    //如果物品有效果数据，则取消订阅玩家的事件，以便在玩家触发相关事件时，物品效果不再生效
    public void RemoveItemEffect() => itemEffect?.Unsubscribe();

    private EquipmentDataSO EquipmentData()
    {
        if (itemData is EquipmentDataSO equipmentData)
            return equipmentData;//如果物品数据是装备数据，则返回装备数据

        return null;
    }

    public bool CanAddStack() => stackSize < itemData.maxStackSize;//判断是否可以添加堆叠数量
    public void AddStack() => stackSize++;//添加堆叠数量
    public void RemoveStack() => stackSize--;//移除堆叠数量

    public string GetItemInfo()
    {
        StringBuilder sb = new StringBuilder();//StringBuilder是一个可变的字符串类，可以高效地构建字符串

        if (itemData.itemType == ItemType.Material)
        {
            sb.AppendLine("");
            sb.AppendLine("材料道具");
            sb.AppendLine("");
            sb.AppendLine("");
            return sb.ToString();
        }

        if (itemData.itemType == ItemType.Consumable)
        {
            sb.AppendLine("");
            sb.AppendLine(itemEffect != null ? itemEffect.effectDescription : "");//避免空引用
            sb.AppendLine("");
            sb.AppendLine("");
            return sb.ToString();
        }

        sb.AppendLine("");//换行

        foreach (var mod in modifiers)//遍历物品的属性加成
        {
            string modType = GetStatNameByType(mod.statType); // 改为中文映射
            string modValue = IsPercentageStat(mod.statType) ? mod.value.ToString() + "%" : mod.value.ToString(); // 如果是百分比属性，添加%符号
            sb.AppendLine("+ " + modValue + " " + modType);//将属性加成信息添加到字符串中
        }

        if (itemEffect != null)
        {
            sb.AppendLine("");
            sb.AppendLine("特殊效果:");
            sb.AppendLine(itemEffect.effectDescription);//如果物品有特殊效果，添加效果描述
        }

        sb.AppendLine("");
        sb.AppendLine("");

        return sb.ToString();
    }

    public string GetItemTypeText(ItemType type)
    {
        return type switch
        {
            ItemType.Material => "材料",
            ItemType.Weapon => "武器",
            ItemType.Armor => "防具",
            ItemType.Trinket => "饰品",
            ItemType.Consumable => "消耗品",
            _ => "未知类型"
        };
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
            case StatType.Damage: return "伤害";
            case StatType.CritChance: return "暴击率";
            case StatType.CritPower: return "暴击伤害";
            case StatType.ArmorReduction: return "破甲";

            case StatType.FireDamage: return "火焰伤害";
            case StatType.IceDamage: return "冰霜伤害";
            case StatType.LightningDamage: return "雷电伤害";

            case StatType.IceResistance: return "冰抗";
            case StatType.FireResistance: return "火抗";
            case StatType.LightningResistance: return "雷抗";

            default: return "未知属性";
        }
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
}
