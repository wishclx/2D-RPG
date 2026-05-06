using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private Player player;
    private Inventory_Player inventory;
    private UI_SkillSlot[] skillSlots;

    [SerializeField] private RectTransform healthRect;//血条
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("快捷槽设置")]
    [SerializeField] private float yOffsetQuickItemParent = 150;
    [SerializeField] private Transform quickItemOptionParent;
    private UI_QuickItemSlotOption[] quickItemOptions;
    private UI_QuickItemSlot[] quickItemSlots;


    private void Start()//在awake之前执行，确保在UI_InGame的Start中可以找到Player对象
    {
        quickItemSlots = GetComponentsInChildren<UI_QuickItemSlot>(true);//获取所有子对象中的UI_QuickItemSlot组件

        player = FindFirstObjectByType<Player>();
        player.health.OnHealthUpdate += UpdateHealthBar;//订阅事件，当玩家的生命值更新时调用UpdateHealthBar方法

        inventory = player.inventory;//获取玩家的背包组件
        inventory.OnInventoryChange += UpdateQuickSlotsUI;//订阅事件，当背包发生变化时调用UpdateQuickSlotsUI方法
        inventory.OnQuickSlotUsed += PlayQuickSlotFeedback;//订阅事件，当快捷槽被使用时调用PlayQuickSlotFeedback方法
    }

    //调用对应槽位的SimulateButtonFeedback方法，模拟按钮反馈
    public void PlayQuickSlotFeedback(int slotNumber) => quickItemSlots[slotNumber].SimulateButtonFeedback();

    public void UpdateQuickSlotsUI()//更新快捷栏槽位UI
    {
        Inventory_Item[] quickItems = inventory.quickItems;//获取快捷栏中的物品数组

        for (int i = 0; i < quickItems.Length; i++)
        {
            quickItemSlots[i].UpdateQuickSlotUI(quickItems[i]);//调用对应槽位的UpdateQuickSlotUI方法，传入槽位中的物品信息
        }

    }

    public void OpenQuickItemOptions(UI_QuickItemSlot quickItemSlot, RectTransform targetRect)//打开快捷物品选项界面
    {
        if (quickItemOptions == null)
            quickItemOptions = quickItemOptionParent.GetComponentsInChildren<UI_QuickItemSlotOption>(true);//获取所有子对象中的UI_QuickItemSlotOption组件

        //从背包中找到所有类型为Consumable的物品
        List<Inventory_Item> consumables = inventory.itemList.FindAll(item => item.itemData.itemType == ItemType.Consumable);

        for (int i = 0; i < quickItemOptions.Length; i++)
        {
            if (i < consumables.Count)//如果选项数量小于消耗品数量，则设置选项界面
            {
                quickItemOptions[i].gameObject.SetActive(true);//激活选项界面
                quickItemOptions[i].SetupOption(quickItemSlot, consumables[i]);//设置选项界面，传入快捷物品槽和对应的消耗品
            }
            else
                quickItemOptions[i].gameObject.SetActive(false);//否则，隐藏选项界面
        }

        quickItemOptionParent.position = targetRect.position + Vector3.up * yOffsetQuickItemParent;//将选项界面的位置设置为目标位置加上一个垂直偏移
    }

    public void HideQuickItemOptions() => quickItemOptionParent.position = new Vector3(0, 9999);//将选项界面的位置设置到屏幕外，隐藏选项界面

    public UI_SkillSlot GetSkillSlots(SkillType skillType)//根据技能类型获取对应的技能槽
    {
        if (skillSlots == null)
            skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);

        foreach (var slot in skillSlots)
        {
            if (slot.skillType == skillType)//如果技能槽的技能类型与传入的技能类型匹配，则返回该技能槽
            {
                slot.gameObject.SetActive(true);//激活技能槽的游戏对象
                return slot;
            }
        }

        return null;
    }

    public UI_SkillSlot GetSkillSlotWithoutActivate(SkillType skillType)//只获取槽位，不改变激活状态
    {
        if (skillSlots == null)
            skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);

        foreach (var slot in skillSlots)
        {
            if (slot.skillType == skillType)
                return slot;
        }

        return null;
    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.health.GetCurrentHealth());
        float maxHealth = player.stats.GetMaxHealth();
        float sizeDiffrence = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);//计算当前生命值与血条长度之间的差距

        if (sizeDiffrence > .1f)
            //如果差距大于0.1，则将血条长度设置为最大生命值，确保血条长度与最大生命值保持一致
            healthRect.sizeDelta = new Vector2(maxHealth + 150, healthRect.sizeDelta.y);


        healthText.text = $"{currentHealth} / {maxHealth}";
        healthSlider.value = player.health.GetHealthPercent();//获取百分比生命
    }
}
