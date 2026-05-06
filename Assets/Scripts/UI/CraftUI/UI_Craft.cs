using UnityEngine;

public class UI_Craft : MonoBehaviour
{
    [SerializeField] private UI_ItemSlotParent inventoryParent;//显示玩家背包物品的UI组件
    private Inventory_Player inventory;//玩家的背包对象

    private UI_CraftPreviw craftPreviwUI;//显示制作预览的UI组件
    private UI_CraftSlot[] craftSlots;//制作品槽位
    private UI_CraftListButton[] craftListButtons;//制作品列表按钮

    public void SetupCraftUI(Inventory_Storage storage)
    {
        inventory = storage.playerInventory;//获取玩家的背包对象
        inventory.OnInventoryChange += UpdateUI;//订阅库存变化事件，当库存发生变化时调用UpdateUI方法更新UI显示
        UpdateUI();//初始更新UI显示

        craftPreviwUI = GetComponentInChildren<UI_CraftPreviw>();//获取子对象中的UI_CraftPreviw组件
        craftPreviwUI.SetupCraftPreviw(storage);//设置制作预览UI，传入库存存储对象
        SetupCraftListButtons();//设置制作品列表按钮
    }

    private void SetupCraftListButtons()
    {
        craftSlots = GetComponentsInChildren<UI_CraftSlot>(true);//获取所有子对象中的UI_CraftSlot组件（包含未激活对象）
        craftListButtons = GetComponentsInChildren<UI_CraftListButton>(true);//获取所有子对象中的UI_CraftListButton组件（包含未激活对象）

        foreach (var slots in craftSlots)
            slots.gameObject.SetActive(false);//将所有制作品槽位设置为不活跃状态

        foreach (var button in craftListButtons)
            button.SetCraftSlots(craftSlots);//将制作品槽位传递给每个制作品列表按钮
    }

    private void UpdateUI() => inventoryParent.UpdateSlots(inventory.itemList);//当库存发生变化时，更新显示玩家背包物品的UI组件
}