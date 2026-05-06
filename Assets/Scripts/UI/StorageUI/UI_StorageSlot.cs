using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StorageSlot : UI_ItemSlot
{
    private Inventory_Storage storage;

    public enum StorageSlotType//仓库槽位类型
    {
        StorageSlot,
        PlayerInventorySlot,
    }

    public StorageSlotType slotType;
    public void SetStorage(Inventory_Storage storage) => this.storage = storage;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        bool transferFullStack = Input.GetKey(KeyCode.LeftControl);//按住左Ctrl键可以一次性转移整叠物品

        if (slotType == StorageSlotType.StorageSlot)
            storage.FromStorageToPlayer(itemInSlot, transferFullStack);//从仓库移到玩家背包

        if (slotType == StorageSlotType.PlayerInventorySlot)
            storage.FromPlayerToStorage(itemInSlot, transferFullStack);

        ui.itemToolTip.ShowToolTip(false, null);//关闭物品提示
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        ui.itemToolTip.ShowToolTip(true, rect, itemInSlot, false, false, true);
    }
}
