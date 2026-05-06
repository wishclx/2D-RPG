using System.Collections.Generic;
using UnityEngine;

public class UI_EquipSlotParent : MonoBehaviour
{
    private UI_EquipSlot[] equipSlots;

    public void UpdateEquipmentSlots(List<Inventory_EquipmentSlot> equipList)// 方法用来更新装备槽的显示
    {
        if (equipSlots == null)
            equipSlots = GetComponentsInChildren<UI_EquipSlot>();// 获取所有子对象中的UI_EquipSlot组件

        for (int i = 0; i < equipSlots.Length; i++)
        {
            var playerEquipSlot = equipList[i];// 从传入的装备列表中获取对应的装备槽数据

            if (playerEquipSlot.HasItem() == false)
                equipSlots[i].UpdateSlot(null);// 如果该装备槽没有装备，则更新UI显示为空

            else
                equipSlots[i].UpdateSlot(playerEquipSlot.equipedItem);// 否则更新UI显示为该装备槽中的装备
        }
    }
}
