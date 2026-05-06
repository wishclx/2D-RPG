using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory;

    [SerializeField] private UI_ItemSlotParent inventorySlotParent;
    [SerializeField] private UI_EquipSlotParent equipSlotParent;
    [SerializeField] private TextMeshProUGUI goldText;

    private void Awake()
    {
        inventory = FindFirstObjectByType<Inventory_Player>();
        //订阅Inventory_Player组件的OnInventoryChange事件，当库存发生变化时调用UpdateInventorySlots方法更新UI显示
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();//调用UpdateUI方法更新UI显示
    }

    private void OnEnable()
    {
        if (inventory == null)
            return;

        UpdateUI();//当UI被启用时，调用UpdateUI方法更新UI显示
    }

    private void UpdateUI()
    {
        inventorySlotParent.UpdateSlots(inventory.itemList);//调用UI_ItemSlotParent组件的UpdateSlots方法更新UI显示
        equipSlotParent.UpdateEquipmentSlots(inventory.equipList);//调用UI_EquipSlotParent组件的UpdateEquipmentSlots方法更新UI显示
        goldText.text = inventory.gold.ToString("N0") + "g.";//更新金币显示
    }

}