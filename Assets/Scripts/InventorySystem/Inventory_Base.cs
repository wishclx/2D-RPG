using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Base : MonoBehaviour, ISaveable
{
    protected Player player;
    public event Action OnInventoryChange;//背包变化事件

    public int maxInventorySize = 10;//背包最大容量
    public List<Inventory_Item> itemList = new List<Inventory_Item>();//背包物品列表

    [Header("物品数据库")]
    [SerializeField] protected ItemListDataSO itemDataBase;//物品数据列表，用于在保存和加载时根据物品的saveID找到对应的ItemDataSO对象


    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }

    public void TryUseItem(Inventory_Item itemToUse)
    {
        Inventory_Item consumable = itemList.Find(item => item == itemToUse);//查找背包中是否有要使用的物品

        if (consumable == null)
            return;

        if (consumable.itemEffect.CanBeUsed(player) == false)
            return;

        consumable.itemEffect.ExecuteEffect();//执行物品效果

        if (consumable.stackSize > 1)
            consumable.RemoveStack();//如果物品堆叠数量大于1，减少堆叠数量
        else
            RemoveOneItem(consumable);//如果物品堆叠数量为1，移除物品

        OnInventoryChange?.Invoke();//触发背包变化事件
    }

    public bool CanAddItem(Inventory_Item itemToAdd)
    {
        bool hasStackable = FindStackable(itemToAdd) != null;//检查背包中是否有可以堆叠的物品
        return hasStackable || itemList.Count < maxInventorySize;//如果有可以堆叠的物品或者背包未满，返回true
    }

    public Inventory_Item FindStackable(Inventory_Item itemToAdd)
    {
        //根据物品数据查找背包中的物品，并检查是否可以堆叠
        return itemList.Find(item => item.itemData == itemToAdd.itemData && item.CanAddStack());
    }

    public void AddItem(Inventory_Item itemToAdd)
    {
        Inventory_Item itemInInventory = FindStackable(itemToAdd);//查找背包中是否已经有相同物品

        if (itemInInventory != null)//如果有相同物品且可以堆叠，增加堆叠数量
            itemInInventory.AddStack();//如果有相同物品，增加堆叠数量
        else
            itemList.Add(itemToAdd);//如果没有相同物品，添加新物品到背包

        OnInventoryChange?.Invoke();//触发背包变化事件
    }

    public void RemoveOneItem(Inventory_Item itemToRemove)//移除背包中的一个物品
    {
        Inventory_Item itemInInventory = itemList.Find(item => item == itemToRemove);//查找背包中是否有要移除的物品

        if (itemInInventory.stackSize > 1)//如果物品堆叠数量大于1，减少堆叠数量
            itemInInventory.RemoveStack();
        else
            itemList.Remove(itemInInventory);//如果物品堆叠数量为1，移除物品


        OnInventoryChange?.Invoke();//触发背包变化事件
    }

    public void RemoveFullStack(Inventory_Item itemToRemove)
    {
        for (int i = 0; i < itemToRemove.stackSize; i++)
        {
            RemoveOneItem(itemToRemove);
        }
    }

    public Inventory_Item FindItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item == itemToFind);//根据物品数据查找背包中的物品
    }

    public Inventory_Item FindSameItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item.itemData == itemToFind.itemData);//根据物品数据查找背包中的物品
    }

    public void TriggerUpdateUI() => OnInventoryChange?.Invoke();//触发背包变化事件，更新UI显示

    public virtual void LoadData(GameData gameData)
    {

    }

    public virtual void SaveData(ref GameData data)
    {

    }
}
