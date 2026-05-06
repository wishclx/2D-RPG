using System.Collections.Generic;
using UnityEngine;

public class Player_DropManager : Entity_DropManager
{
    [Header("Player Drop Setup")]
    [Range(0, 100)]
    [SerializeField] private float chanceToLoseItem = 90f;//丢失物品的概率（百分比）
    private Inventory_Player inventory;//玩家的背包组件

    private void Awake()
    {
        inventory = GetComponent<Inventory_Player>();
    }

    public override void DropItems()
    {
        List<Inventory_Item> inventoryCopy = new List<Inventory_Item>(inventory.itemList);//创建背包和装备的副本，以避免在迭代过程中修改原始列表
        List<Inventory_EquipmentSlot> equipCopy = new List<Inventory_EquipmentSlot>(inventory.equipList);

        foreach (var item in inventoryCopy)
        {
            if (Random.Range(0, 100) < chanceToLoseItem)
            {
                CreateItemDrop(item.itemData);//根据物品数据创建掉落物
                inventory.RemoveFullStack(item);
            }
        }

        //爆装备有点阴,所以暂时不爆了,等后续再考虑要不要加上这个功能
        //foreach (var equip in equipCopy)
        //{
        //    if (Random.Range(0, 100) < chanceToLoseItem && equip.HasItem())//只有当装备槽有物品时才考虑掉落 
        //    {
        //        var item = equip.GetEquipedItem();

        //        CreateItemDrop(item.itemData);//根据物品数据创建掉落物
        //        inventory.UnequipItem(item);//从装备槽中移除物品
        //        inventory.RemoveFullStack(item);//从背包中移除物品 
        //    }
        //}
    }
}