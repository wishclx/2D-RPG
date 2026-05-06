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
    public UI_TreeNode[] neededNodes;
    public UI_TreeNode[] conflictNodes;//与当前技能互斥的节点
    public bool isUnlocked;
    public bool isLocked;

    [Header("Skill details")]
    public SkillDataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private int skillCost;
    [SerializeField] private string lockedColorHex = "#9F9797";
    private Color originalColor;//图标原始颜色

    private void Start()
    {
        if (isUnlocked == false)
            UpdateIconColor(GetColorByHex(lockedColorHex));


        UnlockDefaultSkill();
    }

    public void UnlockDefaultSkill()
    {
        GetNeededComponents();

        // 默认解锁也要尊重互斥与锁定关系，避免互斥技能被同时打开
        if (skillData.unlockedByDefault && CanUnlockAsDefault())
            Unlock();
    }

    // 新增：默认解锁专用判定（不依赖技能点）
    private bool CanUnlockAsDefault()
    {
        if (isLocked || isUnlocked)
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

    private void GetNeededComponents()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>(true);
        connectHandle = GetComponent<UI_TreeConnectHandle>();
    }


    public void Refund()//退还技能点并重置节点状态
    {
        if (isUnlocked == false || skillData.unlockedByDefault)
            return;

        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));

        skillTree.AddSkillPoints(skillData.cost);
        connectHandle.UnlockConnectionImage(false);
    }
    private void Unlock()
    {
        if (isUnlocked)
        {
            UpdateIconColor(Color.white);
            Debug.Log("技能 " + skillData.displayName + " 已经解锁了，无法再次解锁。");
            return;//如果已经解锁，则不执行任何操作
        }

        isUnlocked = true;
        UpdateIconColor(Color.white);//已解锁节点显示白色
        LockConflictNodes();

        skillTree.RemoveSkillPoints(skillData.cost);
        connectHandle.UnlockConnectionImage(true);

        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData);
    }

    public void UnlockWithSaveData()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);//已解锁节点显示白色
        LockConflictNodes();
        connectHandle.UnlockConnectionImage(true);
    }


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
            ui.skillToolTip.LockedSkillEffect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, skillData, this);

        if (isUnlocked || isLocked)
            return;

        ToggleNodeHighlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);
        ui.skillToolTip.StopLockSkillEffect();//确保离开节点时停止锁定效果

        if (isUnlocked || isLocked)
            return;

        ToggleNodeHighlight(false);
    }

    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * .9f;
        highlightColor.a = 1f;
        Color colorToApply = highlight ? highlightColor : originalColor;

        UpdateIconColor(colorToApply);
    }

    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);//将十六进制字符串转换为 Color

        return color;
    }

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



