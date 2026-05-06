using TMPro;
using UnityEngine;

public class UI_Merchant : MonoBehaviour
{
    private Inventory_Player inventory;
    private Inventory_Merchant merchant;

    [SerializeField] private TextMeshProUGUI goldText;
    [Space]
    [SerializeField] private UI_ItemSlotParent merchantsSlots;
    [SerializeField] private UI_ItemSlotParent inventorySlots;
    [SerializeField] private UI_EquipSlotParent equipSlots;//商店UI中显示玩家装备的UI组件

    public void SetupMerchantUI(Inventory_Merchant merchant, Inventory_Player inventory)
    {
        this.merchant = merchant;
        this.inventory = inventory;

        this.inventory.OnInventoryChange += UpdateSlotUI;
        this.merchant.OnInventoryChange += UpdateSlotUI;// 关键：监听商店库存变化
        UpdateSlotUI();

        UI_MerchantSlot[] merchantSlots = GetComponentsInChildren<UI_MerchantSlot>();//获取商店物品栏的所有UI槽位

        foreach (var slot in merchantSlots)
            slot.SetupMerchantUI(merchant);//为每个商店物品栏UI槽位设置商店数据

    }

    private void UpdateSlotUI()
    {
        if (inventory == null)
            return;

        inventorySlots.UpdateSlots(inventory.itemList);//更新玩家物品栏UI
        merchantsSlots.UpdateSlots(merchant.itemList);//更新商店物品栏UI
        equipSlots.UpdateEquipmentSlots(inventory.equipList);//更新玩家装备栏UI

        goldText.text = inventory.gold.ToString("N0") + "g.";//更新金币显示
    }
}
