using System.Text;
using TMPro;
using UnityEngine;

public class UI_ItemToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemInfo;

    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private Transform merchantInfo1;//商店界面商品信息
    [SerializeField] private Transform merchantInfo2;//商店界面背包物品信息
    [SerializeField] private Transform storageItemInfo;//仓库界面物品信息
    [SerializeField] private Transform inventoryInfo;//背包信息UI组件的父对象

    public void ShowToolTip(bool show, RectTransform targetRect, Inventory_Item itemToShow, bool buy = false, bool showMerchantInfo = false, bool showStorageInfo = false)//重载
    {
        base.ShowToolTip(show, targetRect);

        merchantInfo1.gameObject.SetActive(showMerchantInfo && !buy);//显示商品信息
        merchantInfo2.gameObject.SetActive(showMerchantInfo && buy);
        inventoryInfo.gameObject.SetActive(!showMerchantInfo && !showStorageInfo);
        storageItemInfo.gameObject.SetActive(showStorageInfo);//显示仓库物品信息

        int price = buy ? itemToShow.buyPrice : itemToShow.sellPrice;//根据参数选择显示购买价格还是出售价格
        int totalPrice = price * itemToShow.stackSize;//计算总价格

        string priceLabel = buy ? "购买价" : "售价";//商店栏显示购买价，背包栏显示售价
        string fullStackPrice = $"{priceLabel}: {price} x {itemToShow.stackSize} -- {totalPrice}g.";
        string singlePrice = $"{priceLabel}: {price}g.";

        itemPrice.text = itemToShow.stackSize > 1 ? fullStackPrice : singlePrice;//根据堆叠数量选择显示格式 
        itemType.text = itemToShow.GetItemTypeText(itemToShow.itemData.itemType);
        itemInfo.text = itemToShow.GetItemInfo();//调用Inventory_Item的GetItemInfo方法获取物品信息文本

        string color = GetColorByRarity(itemToShow.itemData.itemRarity);
        itemName.text = GetColoredText(color, itemToShow.itemData.itemName);
    }

    private string GetColorByRarity(int rarity)
    {
        if (rarity <= 100) return "white";//普通
        if (rarity <= 300) return "green"; //优秀
        if (rarity <= 600) return "blue";//精良
        if (rarity <= 850) return "purple";//史诗
        return "orange";//传说
    }
}
