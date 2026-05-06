using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    public Inventory_Player playerInventory { get; private set; }
    public List<Inventory_Item> materialStash;//存放材料的仓库


    public void CraftItem(Inventory_Item itemToCraft)
    {
        ConsumedMaterials(itemToCraft);//消耗制作这个物品所需的材料
        playerInventory.AddItem(itemToCraft);//将制作好的物品添加到玩家背包中
    }

    public bool CanCraftItem(Inventory_Item itemToCraft)//检查是否可以制作物品
    {
        return HasEnoughMaterials(itemToCraft) && playerInventory.CanAddItem(itemToCraft);
    }

    public void ConsumedMaterials(Inventory_Item itemToCraft)//在制作物品时消耗所需材料
    {
        foreach (var requiredItem in itemToCraft.itemData.craftRecipe)//遍历物品的制作配方中的每种所需材料
        {
            int amountToConsume = requiredItem.stackSize;//获取配方中要求的数量

            //首先从玩家背包中消耗所需材料，更新剩余需要消耗的数量
            amountToConsume = amountToConsume - CousumedMaterialsAmount(playerInventory.itemList, requiredItem);

            if (amountToConsume > 0)
                //如果玩家背包中的数量不足以满足需求，继续从仓库中消耗所需材料，更新剩余需要消耗的数量
                amountToConsume = amountToConsume - CousumedMaterialsAmount(itemList, requiredItem);

            if (amountToConsume > 0)
                //如果仓库中的数量仍然不足以满足需求，继续从材料仓库中消耗所需材料，更新剩余需要消耗的数量
                amountToConsume = amountToConsume - CousumedMaterialsAmount(materialStash, requiredItem);
        }
    }

    private int CousumedMaterialsAmount(List<Inventory_Item> itemList, Inventory_Item neededItem)//计算在制作过程中消耗的某种材料的数量
    {
        int amountNeeded = neededItem.stackSize;
        int consumedAmount = 0;

        foreach (var item in itemList)
        {
            if (item.itemData != neededItem.itemData)
                continue;//如果当前物品不是所需的材料，跳过

            //计算从当前物品中可以消耗的数量，不能超过物品的堆叠数量，也不能超过剩余需要的数量
            int removeAmount = Mathf.Min(item.stackSize, amountNeeded - consumedAmount);
            item.stackSize -= removeAmount;
            consumedAmount += removeAmount;//更新已消耗的数量

            if (item.stackSize <= 0)
                itemList.Remove(item);//如果当前物品的堆叠数量已经为0或以下，从列表中移除这个物品

            if (consumedAmount >= amountNeeded)
                break;//如果已经消耗的数量满足了所需数量，停止循环
        }

        return consumedAmount;
    }


    private bool HasEnoughMaterials(Inventory_Item itemToCraft)
    {
        foreach (var requiredMaterial in itemToCraft.itemData.craftRecipe)//遍历物品的制作配方中的每种所需材料
        {
            if (GetAvailableAmountOf(requiredMaterial.itemData) < requiredMaterial.stackSize)
                return false;//如果仓库中某种所需材料的可用数量小于配方中要求的数量，返回false
        }

        return true;
    }

    public int GetAvailableAmountOf(ItemDataSO requiredItem)//获取仓库中某种物品的可用数量
    {
        int amount = 0;

        foreach (var item in playerInventory.itemList)//首先检查玩家背包中是否有这种物品，如果有，累加它的数量
        {
            if (item.itemData == requiredItem)
                amount += item.stackSize;
        }

        foreach (var item in itemList)//然后检查仓库中是否有这种物品，如果有，累加它的数量
        {
            if (item.itemData == requiredItem)
                amount += item.stackSize;
        }

        foreach (var item in materialStash)//最后检查材料仓库中是否有这种物品，如果有，累加它的数量
        {
            if (item.itemData == requiredItem)
                amount += item.stackSize;//如果找到与所需物品相同类型的物品，累加它的堆叠数量
        }

        return amount;//返回总数量
    }

    public void AddMaterialToStash(Inventory_Item itemToAdd)//将材料添加到仓库
    {
        var stackableItem = StackableInStash(itemToAdd);

        if (stackableItem != null)
            stackableItem.AddStack();//如果找到可以叠加的物品，叠加它
        else
        {
            var newItemToAdd = new Inventory_Item(itemToAdd.itemData);//如果没有找到可以叠加的物品，创建一个新的物品实例以便添加到仓库
            materialStash.Add(newItemToAdd);//将新的物品添加到材料仓库中
        }

        TriggerUpdateUI();
        materialStash = materialStash.OrderBy(item => item.itemData.itemName).ToList();//将材料仓库中的物品按名称排序以便更好地组织显示
    }

    public Inventory_Item StackableInStash(Inventory_Item itemToAdd)
    {
        //在材料仓库中查找是否有与要添加的物品相同类型且可以叠加的物品，如果找到，返回这个物品，否则返回null
        return materialStash.Find(item => item.itemData == itemToAdd.itemData && item.CanAddStack());
    }

    public void SetInventory(Inventory_Player inventory) => this.playerInventory = inventory;//传递玩家背包的引用

    public void FromPlayerToStorage(Inventory_Item item, bool transferFullStack)//从玩家背包转移物品到仓库
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;//如果选择转移整叠，设置转移数量为物品的堆叠数量，否则设置为1

        for (int i = 0; i < transferAmount; i++)
        {
            if (CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);//创建一个新的物品实例以便转移

                playerInventory.RemoveOneItem(item);
                AddItem(itemToAdd);//将物品从玩家背包移除并添加到仓库
            }
        }

        TriggerUpdateUI();//更新UI显示
    }

    public void FromStorageToPlayer(Inventory_Item item, bool transferFullStack)//从仓库转移物品到玩家背包
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;//如果选择转移整叠，设置转移数量为物品的堆叠数量，否则设置为1

        for (int i = 0; i < transferAmount; i++)
        {
            if (playerInventory.CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);

                RemoveOneItem(item);
                playerInventory.AddItem(itemToAdd);//将物品从仓库移除并添加到玩家背包
            }
        }

        TriggerUpdateUI();
    }

    public override void SaveData(ref GameData data)
    {
        base.SaveData(ref data);

        data.storageItems.Clear();//清空之前保存的仓库物品数据

        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                //获取物品的saveID作为键，如果GameData中的storageItems字典中还没有这个物品的saveID作为键
                //就先添加一个新的键值对，初始数量为0，然后将物品的堆叠数量累加到这个键对应的值上
                string saveID = item.itemData.saveID;

                //如果GameData中的storageItems字典中还没有这个物品的saveID作为键，就先添加一个新的键值对，初始数量为0
                if (data.storageItems.ContainsKey(saveID) == false)
                    data.storageItems[saveID] = 0;

                data.storageItems[saveID] += item.stackSize;
            }
        }

        data.storageMaterials.Clear();

        foreach (var item in materialStash)
        {
            if (item != null && item.itemData != null)
            {
                //获取物品的saveID作为键，如果GameData中的storageMaterials字典中还没有这个物品的saveID作为键
                //就先添加一个新的键值对，初始数量为0，然后将物品的堆叠数量累加到这个键对应的值上
                string saveID = item.itemData.saveID;

                //如果GameData中的storageMaterials字典中还没有这个物品的saveID作为键，就先添加一个新的键值对，初始数量为0
                if (data.storageMaterials.ContainsKey(saveID) == false)
                    data.storageMaterials[saveID] = 0;

                data.storageMaterials[saveID] += item.stackSize;
            }
        }
    }

    public override void LoadData(GameData data)
    {
        itemList.Clear();//清空之前加载的仓库物品数据
        materialStash.Clear();

        foreach (var entry in data.storageItems)
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
                //根据ItemDataSO对象创建一个新的Inventory_Item对象，并将其添加到背包中
                Inventory_Item itemToLoad = new Inventory_Item(itemData);

                AddItem(itemToLoad);
            }
        }

        foreach (var entry in data.storageMaterials)
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
                //根据ItemDataSO对象创建一个新的Inventory_Item对象，并将其添加到材料仓库中
                Inventory_Item itemToLoad = new Inventory_Item(itemData);

                AddMaterialToStash(itemToLoad);
            }
        }
    }
}
