using UnityEngine;

public class UI_Storage : MonoBehaviour
{
    private Inventory_Player inventory;
    private Inventory_Storage storage;
    [SerializeField] private UI_ItemSlotParent inventoryParent;
    [SerializeField] private UI_ItemSlotParent storageParent;
    [SerializeField] private UI_ItemSlotParent materialStashParent;

    public void SetupStorageUI(Inventory_Storage storage)
    {
        this.storage = storage;
        inventory = storage.playerInventory;// 获取玩家库存引用
        storage.OnInventoryChange += UpdataUI;// 订阅事件，当仓库物品发生变化时更新UI
        UpdataUI();

        UI_StorageSlot[] storageSlots = GetComponentsInChildren<UI_StorageSlot>();

        foreach (var slot in storageSlots)
            slot.SetStorage(storage);// 将仓库对象传递给每个UI_StorageSlot，以便它们能够正确地显示和交互
    }

    private void OnEnable()
    {
        UpdataUI();// 当UI被启用时更新显示内容，确保UI显示的是最新的库存和仓库状态
    }

    private void UpdataUI()
    {
        if (storage == null || inventory == null)
            return;// 如果仓库或玩家库存引用为空，直接返回，避免发生错误

        inventoryParent.UpdateSlots(inventory.itemList);// 更新玩家库存UI显示
        storageParent.UpdateSlots(storage.itemList);// 更新仓库UI显示
        materialStashParent.UpdateSlots(storage.materialStash);// 更新材料仓库UI显示
    }
}
