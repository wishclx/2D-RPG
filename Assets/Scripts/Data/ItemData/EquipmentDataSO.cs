using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Equipment item", fileName = "Equipment Data - ")]

public class EquipmentDataSO : ItemDataSO
{
    [Header("Item modifiers")]
    public ItemModifier[] modifiers;//表示这个装备的属性加成值，可以有多个属性加成值，例如攻击力加成、防御力加成等
}

[Serializable]
public class ItemModifier//包含了一个属性modifierValue，表示这个装备的属性加成值
{
    public StatType statType;//表示这个属性加成值对应的属性类型，例如攻击力、防御力、生命值等
    public float value;
}