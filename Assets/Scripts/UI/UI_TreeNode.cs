using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectHandle connectHandle;

    [Header("Unlock details")]
    public UI_TreeNode[] neededNodes;//解锁当前技能需要的技能节点，如果这些技能节点没有被解锁，那么当前技能就不能被解锁
    public UI_TreeNode[] conflictNodes;//与当前技能冲突的技能节点，如果这些技能节点已经被解锁了，那么当前技能就不能被解锁
    public bool isUnlocked;
    public bool isLocked;

    [Header("Skill details")]
    public Skill_DataSO skillData;//技能数据
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;//技能图标
    [SerializeField] private int skillCost;//技能点消耗
    [SerializeField] private string lockedColorHex = "#9F9797"; // 锁定状态的颜色
    private Color originalColor;//原始颜色


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();
        connectHandle = GetComponent<UI_TreeConnectHandle>();

        UpdateIconColor(GetColorByHex(lockedColorHex));//初始化技能图标颜色为锁定状态的颜色
    }

    private void Unlock()
    {
        isUnlocked = true;//解锁技能
        UpdateIconColor(Color.white);//解锁后将技能图标颜色设置为白色
        LockConflictNodes();

        skillTree.RemoveSkillPoints(skillData.cost);//从技能树中扣除技能点
        connectHandle.UnlockConnectionImage(true);//通知连接点连接线已经解锁了，可以将连接线的颜色设置为白色

    }


    public void Refund()
    {
        isUnlocked = false;//取消解锁技能
        isLocked = false;//取消锁定技能
        UpdateIconColor(GetColorByHex(lockedColorHex));//将技能图标颜色恢复为锁定状态的颜色

        skillTree.AddSkillPoints(skillData.cost);//将技能点返还给技能树
        connectHandle.UnlockConnectionImage(false);//通知连接点连接线已经被取消解锁了，可以将连接线的颜色设置为锁定状态的颜色

        //重置技能树中与当前技能相关的技能节点的状态
    }

    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked)//如果技能已经被锁定或者已经解锁了，那么就不能解锁
            return false;

        if (skillTree.EnoughSkillPoints(skillData.cost) == false)//如果技能点不足，那么就不能解锁
            return false;

        foreach (var node in neededNodes)
        {
            if (node.isUnlocked == false)
                return false;
        }

        foreach (var node in conflictNodes)
        {
            if (node.isUnlocked)//如果与当前技能冲突的技能节点已经被解锁了，那么就不能解锁当前技能
                return false;
        }

        return true;
    }

    private void LockConflictNodes()
    {
        foreach (var node in conflictNodes)
            node.isLocked = true;//将与当前技能冲突的技能节点锁定
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

        originalColor = skillIcon.color;//保存原始颜色
        skillIcon.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
            Unlock();

        else if (isLocked)
            ui.skillToolTip.LockedSkillEffect();//如果技能已经被解锁了，那么就播放锁定技能的提示效果，提醒玩家这个技能已经被解锁了
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, this);//显示技能工具提示，并传入技能数据以显示技能信息

        if (isUnlocked==false || isLocked==false)
            ToggleNodeHighlight(true);//如果技能没有被解锁或者没有被锁定，那么就高亮技能图标，提示玩家这个技能可以被解锁了
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);//隐藏技能工具提示

        if (isUnlocked == false || isLocked == false)
            ToggleNodeHighlight(false);//如果技能没有被解锁或者没有被锁定，那么就取消高亮技能图标，恢复技能图标的原始颜色
    }

    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * .9f;
        highlightColor.a = 1f;//将颜色的alpha值设置为1，确保技能图标在鼠标悬停时完全可见
        Color colorToApply=highlight ? highlightColor : originalColor;//如果需要高亮，那么就使用高亮颜色，否则使用原始颜色

        UpdateIconColor(colorToApply);
    }

    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);//将十六进制颜色字符串转换为Color对象

        return color;
    }

    private void OnDisable()
    {
        if(isLocked)
            UpdateIconColor(GetColorByHex(lockedColorHex));
        //在技能节点被禁用时，如果技能处于锁定状态，那么就将技能图标颜色设置为锁定状态的颜色，以确保技能图标在技能树界面关闭时显示正确的颜色
    
        if(isUnlocked)
            UpdateIconColor(Color.white);
        //在技能节点被禁用时，如果技能处于解锁状态，那么就将技能图标颜色设置为白色，以确保技能图标在技能树界面关闭时显示正确的颜色
    }

    private void OnValidate()//在编辑器中修改技能数据时自动更新技能名称和图标
    {
        if (skillData == null)
            return;

        skillName = skillData.displayName;
        skillIcon.sprite = skillData.icon;
        skillCost = skillData.cost;
        gameObject.name = "UI_TreeNode - " + skillData.displayName;//将游戏对象的名字设置为技能名称，方便在层级视图中识别
    }
}
