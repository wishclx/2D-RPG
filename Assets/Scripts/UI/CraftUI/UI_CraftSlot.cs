using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftSlot : MonoBehaviour
{
    private ItemDataSO itemToCraft;//要制作的物品数据
    [SerializeField] private UI_CraftPreviw craftPreviw;//显示制作预览的UI组件

    [SerializeField] private Image craftItemIcon;//显示物品图标的UI组件
    [SerializeField] private TextMeshProUGUI craftItemName;//显示物品名称的UI组件

    public void SetupButton(ItemDataSO craftData)
    {
        this.itemToCraft = craftData;
        this.craftItemIcon.sprite = craftData.itemIcon;
        craftItemName.text = craftData.itemName;//设置按钮显示的物品图标和名称
    }

    public void UpdateCraftPreviw() => craftPreviw.UpdateCraftPreviw(itemToCraft);//当按钮被点击时，更新制作预览UI显示当前要制作的物品信息
}
