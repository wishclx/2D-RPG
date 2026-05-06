using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MerchantSlot : UI_ItemSlot
{
    private Inventory_Merchant merchant;
    public enum MerchantSlotType { MerchantSlot, PlayerSlot }
    public MerchantSlotType slotType;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        bool rightButton = eventData.button == PointerEventData.InputButton.Right;//右键点击
        bool leftButton = eventData.button == PointerEventData.InputButton.Left;//左键点击

        if (slotType == MerchantSlotType.PlayerSlot)//玩家背包物品点击
        {
            if (rightButton)
            {
                bool sellFullStack = Input.GetKey(KeyCode.LeftControl);//按住左Ctrl键，右键点击玩家背包物品，出售整叠物品
                merchant.TrySellItem(itemInSlot, sellFullStack);
            }
            else if (leftButton)
            {
                base.OnPointerDown(eventData);//玩家背包物品左键点击，正常操作
            }

        }
        else if (slotType == MerchantSlotType.MerchantSlot)//商人物品点击
        {
            if (rightButton)
                return;//商人物品右键点击，不做任何操作

            bool buyFullStack = Input.GetKey(KeyCode.LeftControl);//按住左Ctrl键，左键点击商人物品，购买整叠物品
            merchant.TryBuyItem(itemInSlot, buyFullStack);
        }

        ui.itemToolTip.ShowToolTip(false, null);//点击后隐藏物品提示
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        if (slotType == MerchantSlotType.MerchantSlot)
            ui.itemToolTip.ShowToolTip(true, rect, itemInSlot, true, true);//商人物品显示购买价格
        else
            ui.itemToolTip.ShowToolTip(true, rect, itemInSlot, false, true);//玩家背包物品显示正常提示
    }

    public void SetupMerchantUI(Inventory_Merchant merchant) => this.merchant = merchant;
}
