using System.Collections.Generic;
using UnityEngine;

public class Inventory_Merchant : Inventory_Base
{
    private Inventory_Player inventory;

    [SerializeField] private ItemListDataSO shopData;//商店物品数据
    [SerializeField] private int minItemsAmount = 4;//商店物品最小数量

    protected override void Awake()
    {
        base.Awake();
        FillShopList();
    }

    public void TryBuyItem(Inventory_Item itemToBuy, bool buyFullStack)
    {
        int amoutToBuy = buyFullStack ? itemToBuy.stackSize : 1;//确定购买数量

        for (int i = 0; i < amoutToBuy; i++)
        {
            if (inventory.gold < itemToBuy.buyPrice)
            {
                Debug.Log("没有足够的金币");
                return;
            }

            if (itemToBuy.itemData.itemType == ItemType.Material)
            {
                var materialToAdd = new Inventory_Item(itemToBuy.itemData);//使用新实例，避免与商店列表共享引用
                inventory.storage.AddMaterialToStash(materialToAdd);//如果是材料，添加到材料仓库
            }
            else
            {
                if (inventory.CanAddItem(itemToBuy))
                {
                    var itemToAdd = new Inventory_Item(itemToBuy.itemData);//创建一个新的Inventory_Item实例
                    inventory.AddItem(itemToAdd);//将物品添加到玩家的背包
                }
            }

            inventory.gold -= itemToBuy.buyPrice;//扣除金币
            RemoveOneItem(itemToBuy);//从商店物品列表中移除购买的物品
        }

        TriggerUpdateUI(); //更新UI显示
    }

    public void TrySellItem(Inventory_Item itemToSell, bool sellFullStack)
    {
        int amoutToSell = sellFullStack ? itemToSell.stackSize : 1;//确定出售数量

        for (int i = 0; i < amoutToSell; i++)
        {
            int sellPrice = itemToSell.sellPrice;//获取物品的出售价格

            inventory.gold += sellPrice;//增加金币
            inventory.RemoveOneItem(itemToSell);
        }

        TriggerUpdateUI(); //更新UI显示
    }

    public void FillShopList()
    {
        itemList.Clear();//清空商店物品列表
        List<Inventory_Item> possibleItems = new List<Inventory_Item>();

        foreach (var itemData in shopData.itemList)
        {
            int randomziedStack = Random.Range(itemData.minStackSizeAtShop, itemData.maxStackSizeAtShop + 1);//随机生成物品堆叠数量
            int finalStack = Mathf.Clamp(randomziedStack, 1, itemData.maxStackSize);//确保堆叠数量不超过物品最大堆叠数量

            Inventory_Item itemToAdd = new Inventory_Item(itemData);//创建新的Inventory_Item实例
            itemToAdd.stackSize = finalStack;//设置堆叠数量

            possibleItems.Add(itemToAdd);//将物品添加到可能的物品列表中
        }

        int randomItemsAmount = Random.Range(minItemsAmount, maxInventorySize + 1);//随机生成商店物品数量
        int finalAmount = Mathf.Clamp(randomItemsAmount, 1, possibleItems.Count);//确保商店物品数量不超过可能的物品数量

        for (int i = 0; i < finalAmount; i++)
        {
            var randomIndex = Random.Range(0, possibleItems.Count);//随机选择一个物品
            var item = possibleItems[randomIndex];//获取该物品

            if (CanAddItem(item))
            {
                possibleItems.Remove(item);//从可能的物品列表中移除该物品
                AddItem(item);//将该物品添加到商店物品列表中
            }
        }

        TriggerUpdateUI(); //更新UI显示
    }


    public void SetInventory(Inventory_Player inventory) => this.inventory = inventory;
}
