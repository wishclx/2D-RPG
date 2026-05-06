using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory_Item itemInSlot { get; private set; }
    protected Inventory_Player inventory;
    protected UI ui;
    protected RectTransform rect;//槽位的RectTransform组件

    [Header("UI槽位设置")]
    [SerializeField] protected GameObject defaultIcon;
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected TextMeshProUGUI itemStackSize;

    protected virtual void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        inventory = FindAnyObjectByType<Inventory_Player>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)//点击槽位时调用
    {
        bool rightButton = eventData.button == PointerEventData.InputButton.Right;//右键点击
        bool leftButton = eventData.button == PointerEventData.InputButton.Left;//左键点击

        if (rightButton)
            return;//右键点击时不做任何操作

        if (itemInSlot == null || itemInSlot.itemData.itemType == ItemType.Material)
            return;

        bool alternativeInput = Input.GetKey(KeyCode.LeftControl);//按住左Ctrl键时，尝试丢弃物品

        if (alternativeInput)
        {
            inventory.RemoveOneItem(itemInSlot);//尝试丢弃该物品
        }

        else
        {
            if (itemInSlot.itemData.itemType == ItemType.Consumable)
            {
                inventory.TryUseItem(itemInSlot);//尝试使用该物品
            }
            else
                inventory.TryEquipItem(itemInSlot);//尝试装备该物品
        }

        if (itemInSlot == null)
            ui.itemToolTip.ShowToolTip(false, null);//如果物品被装备了，隐藏物品信息提示
    }

    public void UpdateSlot(Inventory_Item item)//更新槽位显示
    {
        itemInSlot = item;//更新槽位显示

        if (defaultIcon != null)
            defaultIcon.gameObject.SetActive(itemInSlot == null);//如果没有物品，显示默认图标，否则隐藏默认图标

        if (itemInSlot == null)
        {
            itemStackSize.text = "";
            itemIcon.color = Color.clear;//如果没有物品，图标变为透明
            return;
        }

        Color color = Color.white;//如果有物品，图标变为不透明
        color.a = .9f;//设置图标的透明度
        itemIcon.color = color;
        itemIcon.sprite = itemInSlot.itemData.itemIcon;//设置图标的图片
        itemStackSize.text = itemInSlot.stackSize > 1 ? itemInSlot.stackSize.ToString() : "";//如果堆叠数量大于1，显示堆叠数量，否则不显示
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        ui.itemToolTip.ShowToolTip(true, rect, itemInSlot);//显示物品信息提示
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemToolTip.ShowToolTip(false, null);//隐藏物品信息提示
    }
}
