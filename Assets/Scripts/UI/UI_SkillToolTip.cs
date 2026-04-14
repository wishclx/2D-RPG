using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// UI_SkillToolTip 的职责说明。
/// </summary>
public class UI_SkillToolTip : UI_ToolTip
{
    private UI ui;
    private UI_SkillTree skillTree;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    [Space]
    [SerializeField] private string metConditionHex;//条件满足时的颜色
    [SerializeField] private string notmetConditionHex;//条件不满足时的颜色
    [SerializeField] private string importantInfoHex; // 重要提示文本颜色。
    [SerializeField] private Color exampleColor; // 颜色预览（用于编辑器调试）。
    [SerializeField] private string lockedSkillText = "已选择另一分支，该技能已锁定";

    private Coroutine textEffectCo;

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<UI_SkillTree>();
    }

    /// <summary>
    /// 执行 ShowToolTip 逻辑。
    /// </summary>
    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    public void ShowToolTip(bool show, RectTransform targetRect, UI_TreeNode node)
    {
        base.ShowToolTip(show, targetRect);

        if (show == false)
            return;

        skillName.text = node.skillData.displayName;
        skillDescription.text = node.skillData.description;

        string skillLockedText = GetColoredText(importantInfoHex, lockedSkillText);
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.skillData.cost, node.neededNodes, node.conflictNodes);

        skillRequirements.text = requirements;
    }

    /// <summary>
    /// 执行 LockedSkillEffect 逻辑。
    /// </summary>
    public void LockedSkillEffect()
    {
        if (textEffectCo != null)
            StopCoroutine(textEffectCo);

        textEffectCo = StartCoroutine(TextBlinkEffectCo(skillRequirements, .15f, 3));
    }

    private IEnumerator TextBlinkEffectCo(TextMeshProUGUI text, float blinkInterval, int blinkCount)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            text.text = GetColoredText(notmetConditionHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);

            text.text = GetColoredText(importantInfoHex, lockedSkillText);// 切回重要提示颜色。
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    /// <summary>
    /// 执行 GetRequirements 逻辑。
    /// </summary>
    private string GetRequirements(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("技能需求：");

        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionHex : notmetConditionHex;
        string costText = $"- {skillCost} 技能点数";
        string finalCostText = GetColoredText(costColor, costText);// 包装为带颜色文本。

        sb.AppendLine(finalCostText);

        foreach (var node in neededNodes)
        {
            if (node == null) continue;// 可能存在空节点，跳过。

            string nodeColor = node.isUnlocked ? metConditionHex : notmetConditionHex;
            string nodeText = $"- {node.skillData.displayName}";
            string finalNodeText = GetColoredText(nodeColor, nodeText);// 包装为带颜色文本。

            sb.AppendLine(finalNodeText);
        }

        if (conflictNodes.Length <= 0)
            return sb.ToString();

        sb.AppendLine();
        sb.AppendLine(GetColoredText(importantInfoHex, "冲突技能： "));

        foreach (var node in conflictNodes)
        {
            if (node == null) continue;// 可能存在空节点，跳过。

            string nodeText = $"- {node.skillData.displayName}";
            string finalNodeText = GetColoredText(importantInfoHex, nodeText);
            sb.AppendLine(finalNodeText);
        }

        return sb.ToString();
    }

}



