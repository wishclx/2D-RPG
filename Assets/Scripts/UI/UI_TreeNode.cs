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
    public UI_TreeNode[] neededNodes;//解锁当前技能所需的前置节点
    public UI_TreeNode[] conflictNodes;//与当前技能互斥的节点
    public bool isUnlocked;
    public bool isLocked;

    [Header("Skill details")]
    public Skill_DataSO skillData;//技能配置数据
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;//技能图标
    [SerializeField] private int skillCost;//技能点消耗
    [SerializeField] private string lockedColorHex = "#9F9797"; // 锁定状态颜色
    private Color originalColor;//图标原始颜色


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();
        connectHandle = GetComponent<UI_TreeConnectHandle>();

        UpdateIconColor(GetColorByHex(lockedColorHex));//初始化为锁定配色
    }

    private void Unlock()
    {
        isUnlocked = true;//标记为已解锁
        UpdateIconColor(Color.white);//已解锁节点显示白色
        LockConflictNodes();

        skillTree.RemoveSkillPoints(skillData.cost);//扣除对应技能点
        connectHandle.UnlockConnectionImage(true);//连接线切换为解锁颜色

        //把本节点的升级效果同步到对应技能实例
        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData.upgradeData);//根据技能类型与升级类型应用强化
    }


    public void Refund()
    {
        isUnlocked = false;//清除解锁状态
        isLocked = false;//清除锁定状态
        UpdateIconColor(GetColorByHex(lockedColorHex));//图标恢复锁定配色

        skillTree.AddSkillPoints(skillData.cost);//返还技能点
        connectHandle.UnlockConnectionImage(false);//连接线恢复未解锁颜色

        // 如有需要，可在这里补充联动节点的状态重置
    }

    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked)//已锁定或已解锁时不可再次解锁
            return false;

        if (skillTree.EnoughSkillPoints(skillData.cost) == false)//技能点不足时不可解锁
            return false;

        foreach (var node in neededNodes)
        {
            if (node.isUnlocked == false)
                return false;
        }

        foreach (var node in conflictNodes)
        {
            if (node.isUnlocked)//存在已解锁冲突技能时不可解锁
                return false;
        }

        return true;
    }

    private void LockConflictNodes()
    {
        foreach (var node in conflictNodes)
            node.isLocked = true;//锁定所有冲突节点
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

        originalColor = skillIcon.color;//缓存图标当前颜色
        skillIcon.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
            Unlock();

        else if (isLocked)
            ui.skillToolTip.LockedSkillEffect();//点击锁定节点时播放锁定提示效果
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, this);//显示技能提示并传入当前节点

        if (isUnlocked || isLocked)
            return;

        ToggleNodeHighlight(true);//仅对可交互节点显示高亮
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);//隐藏技能提示

        if (isUnlocked || isLocked)
            return;

        ToggleNodeHighlight(false);//取消高亮并恢复原颜色
    }

    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * .9f;
        highlightColor.a = 1f;//高亮时保持完全不透明
        Color colorToApply = highlight ? highlightColor : originalColor;//根据状态选择目标颜色

        UpdateIconColor(colorToApply);
    }

    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);//把十六进制字符串转换为 Color

        return color;
    }

    private void OnDisable()
    {
        if (isLocked)
            UpdateIconColor(GetColorByHex(lockedColorHex));
        // 组件禁用时保持锁定节点的显示颜色正确

        if (isUnlocked)
            UpdateIconColor(Color.white);
        // 组件禁用时保持已解锁节点的显示颜色正确
    }

    private void OnValidate()//编辑器参数变更时自动同步显示字段
    {
        if (skillData == null)
            return;

        skillName = skillData.displayName;
        skillIcon.sprite = skillData.icon;
        skillCost = skillData.cost;
        gameObject.name = "UI_TreeNode - " + skillData.displayName;//同步节点名称，便于层级面板识别
    }
}
