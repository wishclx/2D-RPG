using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuickItemSlotOption : UI_ItemSlot
{
    private UI_QuickItemSlot currentQuickItemSlot;

    public void SetupOption(UI_QuickItemSlot currentQuickItemSlot, Inventory_Item itemToSet)//设置快捷栏选项
    {
        this.currentQuickItemSlot = currentQuickItemSlot;
        UpdateSlot(itemToSet);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        currentQuickItemSlot.SetupQuickSlotItem(itemInSlot);//将当前选项的物品设置到快捷栏槽位中
        ui.inGameUI.HideQuickItemOptions();
    }
}
