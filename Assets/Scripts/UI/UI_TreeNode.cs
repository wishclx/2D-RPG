using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI_TreeNode 的职责说明。
/// </summary>
public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectHandle connectHandle;

    [Header("Unlock details")]
    public UI_TreeNode[] neededNodes;
    public UI_TreeNode[] conflictNodes;//与当前技能互斥的节点
    public bool isUnlocked;
    public bool isLocked;

    [Header("Skill details")]
    public Skill_DataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private int skillCost;
    [SerializeField] private string lockedColorHex = "#9F9797";
    private Color originalColor;//图标原始颜色


    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();
        connectHandle = GetComponent<UI_TreeConnectHandle>();

        UpdateIconColor(GetColorByHex(lockedColorHex));

    }

    private void Start()
    {
        if (skillData.unlockedByDefault)
            Unlock();
    }

    /// <summary>
    /// 执行 Unlock 逻辑。
    /// </summary>
    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);//已解锁节点显示白色
        LockConflictNodes();

        skillTree.RemoveSkillPoints(skillData.cost);
        connectHandle.UnlockConnectionImage(true);


        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData.upgradeData);
    }


    /// <summary>
    /// 执行 Refund 逻辑。
    /// </summary>
    public void Refund()//退还技能点并重置节点状态
    {
        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));

        skillTree.AddSkillPoints(skillData.cost);
        connectHandle.UnlockConnectionImage(false);


    }

    /// <summary>
    /// 执行 CanBeUnlocked 逻辑。
    /// </summary>
    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked)
            return false;

        if (skillTree.EnoughSkillPoints(skillData.cost) == false)
            return false;

        foreach (var node in neededNodes)
        {
            if (node.isUnlocked == false)
                return false;
        }

        foreach (var node in conflictNodes)
        {
            if (node.isUnlocked)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 执行 LockConflictNodes 逻辑。
    /// </summary>
    private void LockConflictNodes()
    {
        foreach (var node in conflictNodes)
        {
            node.isLocked = true;
            node.LockChildNodes();//锁定互斥节点的子节点
        }
    }

    public void LockChildNodes()
    {
        isLocked = true;

        foreach (var node in connectHandle.GetChildNodes())
            node.LockChildNodes();//递归锁定子节点
    }

    /// <summary>
    /// 执行 UpdateIconColor 逻辑。
    /// </summary>
    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

        originalColor = skillIcon.color;//缓存图标当前颜色
        skillIcon.color = color;
    }

    /// <summary>
    /// 执行 OnPointerDown 逻辑。
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
            Unlock();

        else if (isLocked)
            ui.skillToolTip.LockedSkillEffect();
    }

    /// <summary>
    /// 执行 OnPointerEnter 逻辑。
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, this);

        if (isUnlocked || isLocked)
            return;

        ToggleNodeHighlight(true);
    }

    /// <summary>
    /// 执行 OnPointerExit 逻辑。
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);

        if (isUnlocked || isLocked)
            return;

        ToggleNodeHighlight(false);
    }

    /// <summary>
    /// 执行 ToggleNodeHighlight 逻辑。
    /// </summary>
    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * .9f;
        highlightColor.a = 1f;
        Color colorToApply = highlight ? highlightColor : originalColor;

        UpdateIconColor(colorToApply);
    }

    /// <summary>
    /// 执行 GetColorByHex 逻辑。
    /// </summary>
    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);//将十六进制字符串转换为 Color

        return color;
    }

    /// <summary>
    /// 执行 OnDisable 逻辑。
    /// </summary>
    private void OnDisable()
    {
        if (isLocked)
            UpdateIconColor(GetColorByHex(lockedColorHex));


        if (isUnlocked)
            UpdateIconColor(Color.white);

    }

    private void OnValidate()
    {
        if (skillData == null)
            return;

        skillName = skillData.displayName;
        skillIcon.sprite = skillData.icon;
        skillCost = skillData.cost;
        gameObject.name = "UI_TreeNode - " + skillData.displayName;
    }
}



