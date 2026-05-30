using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    public event Action<int> OnQuickSlotUsed;//定义一个事件，当快键槽被使用时触发，传递被使用的槽位编号和物品信息

    public Inventory_Storage storage { get; private set; }
    public List<Inventory_EquipmentSlot> equipList;

    [Header("快键槽")]
    public Inventory_Item[] quickItems = new Inventory_Item[2];//快键槽数组，默认大小为2，可以在Inspector中调整

    [Header("金币信息")]
    public int gold = 10000;//玩家初始金币数量

    protected override void Awake()
    {
        base.Awake();
        storage = FindFirstObjectByType<Inventory_Storage>();//在场景中找到第一个Inventory_Storage对象，并将其引用赋值给storage属性
    }

    public void SetQuickItemInSlot(int slotNumber, Inventory_Item itemToSet)
    {
        quickItems[slotNumber - 1] = itemToSet;
        TriggerUpdateUI();//触发UI更新事件，通知UI更新显示
    }

    public void TryUseQuickItemInSlot(int passedSlotNumber)
    {
        int slotNumer = passedSlotNumber - 1;//将传入的槽位编号转换为数组索引
        var itemToUse = quickItems[slotNumer];//获取快键槽中的物品

        if (itemToUse == null)
            return;

        TryUseItem(itemToUse);

        if (FindItem(itemToUse) == null)//如果使用后背包中没有该物品了
        {
            quickItems[slotNumer] = FindSameItem(itemToUse);//尝试在背包中找到相同的物品，如果找到了就放到快键槽里，如果没有找到就将快键槽设置为null
        }

        TriggerUpdateUI();//触发UI更新事件，通知UI更新显示
        OnQuickSlotUsed?.Invoke(slotNumer);//触发快键槽变化事件，通知UI更新显示
    }

    public void TryEquipItem(Inventory_Item item)
    {
        var inventoryItem = FindItem(item);//在背包中寻找该物品
        var matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);//找到所有符合该物品类型的装备槽

        foreach (var slot in matchingSlots)
        {
            if (slot.HasItem() == false)
            {
                EquipItem(inventoryItem, slot);// 如果槽位没有装备，装备该物品
                return;// 如果有空槽，直接返回
            }
        }

        // 如果没有空槽，替换第一个符合条件的槽位
        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equipedItem;

        //先卸下当前装备的物品，再装备新物品。replacingItem参数为true，表示正在替换装备，这样在卸下装备时不会检查背包是否已满。
        UnequipItem(itemToUnequip, slotToReplace != null);
        EquipItem(inventoryItem, slotToReplace);
    }

    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        float savedHealthPercent = player.health.GetHealthPercent();//保存当前生命值百分比

        slot.equipedItem = itemToEquip;//将物品装备到槽位
        slot.equipedItem.AddModifiers(player.stats);//添加物品属性加成
        slot.equipedItem.AddItemEffect(player);//添加物品效果

        player.health.SetHealthToPercent(savedHealthPercent);//根据保存的百分比调整生命值，确保装备更换不会改变当前生命值百分比
        RemoveOneItem(itemToEquip);//从背包中移除该物品
    }

    public void UnequipItem(Inventory_Item itemToUnequip, bool replacingItem = false)//卸下装备
    {
        if (CanAddItem(itemToUnequip) == false && replacingItem == false)
        {
            Debug.Log("背包已满，无法卸下装备");
            return;
        }

        float savedHealthPercent = player.health.GetHealthPercent();

        var slotToUnequip = equipList.Find(slot => slot.equipedItem == itemToUnequip);//找到装备该物品的槽位

        if (slotToUnequip != null)
            slotToUnequip.equipedItem = null;//将槽位的装备设置为null

        itemToUnequip.RemoveModifiers(player.stats);//移除物品属性加成
        itemToUnequip.RemoveItemEffect();//移除物品效果

        player.health.SetHealthToPercent(savedHealthPercent);
        AddItem(itemToUnequip);//将物品添加回背包
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        gold += amount;//增加玩家金币
    }

    public override void SaveData(ref GameData data)
    {
        data.gold = gold;//将玩家的金币数量保存到GameData中
        data.inventory.Clear();//清空GameData中的背包数据
        data.equipItems.Clear();

        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                if (data.inventory.ContainsKey(saveID) == false)
                    data.inventory[saveID] = 0;

                data.inventory[saveID] += item.stackSize;
            }
        }

        foreach (var slot in equipList)
        {
            if (slot.HasItem())
                data.equipItems[slot.equipedItem.itemData.saveID] = slot.slotType;
        }

        // 新增：保存快键槽（保存为槽位索引 -> item saveID）
        if (data.quickSlots == null)
            data.quickSlots = new SerializableDictionary<string, string>();
        data.quickSlots.Clear();
        for (int i = 0; i < quickItems.Length; i++)
        {
            var qi = quickItems[i];
            if (qi != null && qi.itemData != null)
                data.quickSlots[i.ToString()] = qi.itemData.saveID;
        }
    }

    public override void LoadData(GameData data)
    {
        gold = data.gold;//从GameData中加载玩家的金币数量

        foreach (var entry in data.inventory)
        {
            string saveID = entry.Key;
            int stackSize = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveID);//根据saveID从物品数据列表中找到对应的ItemDataSO对象

            if (itemData == null)
            {
                Debug.LogWarning($"无法找到saveID为{saveID}的物品数据，跳过加载该物品");
                continue;
            }

            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData);
                AddItem(itemToLoad);
            }
        }

        foreach (var entry in data.equipItems)
        {
            string saveId = entry.Key;
            ItemType loadedSlotType = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveId);//根据saveID从物品数据列表中找到对应的ItemDataSO对象
            Inventory_Item itemToLoad = new Inventory_Item(itemData);

            var slot = equipList.Find(slot => slot.slotType == loadedSlotType && slot.HasItem() == false);//找到与保存的槽位类型匹配的装备槽 

            slot.equipedItem = itemToLoad;
            slot.equipedItem.AddModifiers(player.stats);//添加物品属性加成
            slot.equipedItem.AddItemEffect(player);//添加物品效果
        }

        // 新增：恢复快键槽
        if (data.quickSlots != null)
        {
            for (int i = 0; i < quickItems.Length; i++)
            {
                string key = i.ToString();
                if (data.quickSlots.TryGetValue(key, out string saveId) && string.IsNullOrEmpty(saveId) == false)
                {
                    ItemDataSO quickItemData = itemDataBase.GetItemData(saveId);
                    if (quickItemData != null)
                    {
                        quickItems[i] = new Inventory_Item(quickItemData);
                    }
                    else
                    {
                        quickItems[i] = null;
                    }
                }
                else
                {
                    quickItems[i] = null;
                }
            }
        }
        else
        {
            // 若无保存数据，清空快键槽
            for (int i = 0; i < quickItems.Length; i++)
                quickItems[i] = null;
        }

        TriggerUpdateUI();//触发UI更新事件，通知UI更新显示
    }
}
