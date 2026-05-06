using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreviw : MonoBehaviour
{
    private Inventory_Item itemToCraft;//当前预览的制作品数据
    private Inventory_Storage storage;
    private UI_CraftPreviwSlot[] craftPreviwSlots;

    [Header("Item Previw Setup")]
    [SerializeField] private Image itemIcon;//预览物品图标
    [SerializeField] private TextMeshProUGUI itemName;//预览物品名称
    [SerializeField] private TextMeshProUGUI itemInfo;//预览物品信息
    [SerializeField] private TextMeshProUGUI buttonText;//制作按钮文本
    [SerializeField] private string craftButtonDefaultText = "制作";//按钮默认文案

    public void SetupCraftPreviw(Inventory_Storage storage)
    {
        this.storage = storage;

        craftPreviwSlots = GetComponentsInChildren<UI_CraftPreviwSlot>();//获取所有子对象中的UI_CraftPreviwSlot组件
        foreach (var slot in craftPreviwSlots)
            slot.gameObject.SetActive(false);//初始时隐藏所有预览槽位
    }

    public void ConfirmCraft()
    {
        if (itemToCraft == null)
        {
            buttonText.text = "选择一个物品!";//如果没有预览的物品，更新按钮文本提示用户
            return;//如果没有预览的物品，直接返回
        }

        if (storage.CanCraftItem(itemToCraft))//如果仓库中有足够的材料，并且玩家背包可以添加这个物品
            storage.CraftItem(itemToCraft);

        UpdateCraftPreviwSlots();//更新预览槽位的显示内容，以反映当前材料的数量变化
    }

    public void UpdateCraftPreviw(ItemDataSO itemData)
    {
        itemToCraft = new Inventory_Item(itemData);//创建一个新的Inventory_Item实例来存储预览数据

        itemIcon.sprite = itemData.itemIcon;
        itemName.text = itemData.itemName;
        itemInfo.text = itemToCraft.GetItemInfo();//调用GetItemInfo方法获取物品的详细信息并显示在UI上
        buttonText.text = craftButtonDefaultText;//选择物品后恢复按钮默认文案

        UpdateCraftPreviwSlots();
    }

    private void UpdateCraftPreviwSlots()
    {
        foreach (var slot in craftPreviwSlots)
            slot.gameObject.SetActive(false);//隐藏所有预览槽位

        for (int i = 0; i < itemToCraft.itemData.craftRecipe.Length; i++)
        {
            Inventory_Item requiredItem = itemToCraft.itemData.craftRecipe[i];//获取当前配方中所需的物品
            int avaliableAmount = storage.GetAvailableAmountOf(requiredItem.itemData);//调用存储系统的方法获取当前拥有的该物品的数量
            int requiredAmount = requiredItem.stackSize;//获取配方中所需的该物品的数量

            craftPreviwSlots[i].gameObject.SetActive(true);//显示当前预览槽位
            //设置预览槽位的显示内容，包括物品图标、名称、拥有数量和所需数量
            craftPreviwSlots[i].SetupMaterialSlot(requiredItem.itemData, avaliableAmount, requiredAmount);
        }
    }
}
