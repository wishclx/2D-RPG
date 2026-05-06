using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_QuickItemSlot : UI_ItemSlot
{
    private Button button;
    [SerializeField] private Sprite defaultSprite;//默认图标
    [SerializeField] private int slotNumber;

    protected override void Awake()
    {
        base.Awake();
        button = GetComponent<Button>();
    }

    public void SetupQuickSlotItem(Inventory_Item itemToPass)//设置快捷栏物品
    {
        inventory.SetQuickItemInSlot(slotNumber, itemToPass);
    }

    public void SimulateButtonFeedback()//模拟按钮反馈
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);
        ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);//触发按钮的点击事件
    }

    public void UpdateQuickSlotUI(Inventory_Item currentItemInSlot)//更新快捷栏槽位显示
    {
        if (currentItemInSlot == null || currentItemInSlot.itemData == null)
        {
            itemIcon.sprite = defaultSprite;
            itemStackSize.text = "";
            return;
        }

        itemIcon.sprite = currentItemInSlot.itemData.itemIcon;
        itemStackSize.text = currentItemInSlot.stackSize.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.inGameUI.OpenQuickItemOptions(this, rect);
    }
}
