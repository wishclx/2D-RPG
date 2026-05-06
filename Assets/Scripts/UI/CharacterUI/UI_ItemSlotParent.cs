using System.Collections.Generic;
using UnityEngine;

public class UI_ItemSlotParent : MonoBehaviour
{
    private UI_ItemSlot[] slots;


    public void UpdateSlots(List<Inventory_Item> itemList)//更新槽位显示
    {
        if (slots == null)
            slots = GetComponentsInChildren<UI_ItemSlot>();//获取子物体上的UI_ItemSlot组件

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < itemList.Count)
            {
                slots[i].UpdateSlot(itemList[i]);//更新槽位显示物品
            }
            else
            {
                slots[i].UpdateSlot(null);//如果没有物品则清空槽位显示
            }
        }
    }
}
