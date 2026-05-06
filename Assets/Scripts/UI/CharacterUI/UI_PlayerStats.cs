using UnityEngine;

public class UI_PlayerStats : MonoBehaviour
{
    private UI_StatSlot[] uiStatSlots;
    private Inventory_Player inventory;

    private void Awake()
    {
        uiStatSlots = GetComponentsInChildren<UI_StatSlot>();

        inventory = FindFirstObjectByType<Inventory_Player>();//找到玩家的Inventory_Player组件
        inventory.OnInventoryChange += UpdateStatUI;//订阅事件，当玩家的装备发生变化时更新UI
    }

    private void Start()
    {
        UpdateStatUI();
    }

    private void UpdateStatUI()
    {
        //更新UI显示
        foreach (var statSlot in uiStatSlots)
            statSlot.UpdateStatValue();//调用每个UI_StatSlot的UpdateStatValue方法来更新显示的数值
    }
}
