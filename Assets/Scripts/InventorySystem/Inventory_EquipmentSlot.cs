using System;
using UnityEngine;

[Serializable]
public class Inventory_EquipmentSlot
{
    public ItemType slotType;
    public Inventory_Item equipedItem;//当前装备的物品

    public Inventory_Item GetEquipedItem() => equipedItem;
    public bool HasItem() => equipedItem != null && equipedItem.itemData != null;//检查槽位是否有物品
}
