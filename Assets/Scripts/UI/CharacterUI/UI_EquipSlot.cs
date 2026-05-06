using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipSlot : UI_ItemSlot
{
    public ItemType slotType;

    private void OnValidate()//编辑器中显示装备类型
    {
        gameObject.name = "UI_EquipmentSlot - " + slotType.ToString();//在编辑器中显示装备类型
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        inventory.UnequipItem(itemInSlot);//尝试卸下该装备
    }
}
