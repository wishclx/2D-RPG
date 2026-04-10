using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    private UI ui;
    private UI_SkillTree skillTree;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;//技能需求文本

    [Space]
    [SerializeField] private string metConditionHex;//条件满足时的颜色
    [SerializeField] private string notmetConditionHex;//条件不满足时的颜色
    [SerializeField] private string importantInfoHex;//重要提示（冲突）颜色
    [SerializeField] private Color exampleColor;//颜色预览（调试用）
    [SerializeField] private string lockedSkillText = "选择了另一分支,此技能已锁定";//技能被锁定时的提示语

    private Coroutine textEffectCo;//文本闪烁协程

    protected override void Awake()
    {
        base.Awake();
        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<UI_SkillTree>();
    }

    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    public void ShowToolTip(bool show, RectTransform targetRect, UI_TreeNode node)//重载：额外传入技能节点数据
    {
        base.ShowToolTip(show, targetRect);//先执行基础显示/隐藏逻辑

        if (show == false)
            return;

        skillName.text = node.skillData.displayName;//显示技能名
        skillDescription.text = node.skillData.description;

        string skillLockedText = GetColoredText(importantInfoHex, lockedSkillText);//按重要颜色渲染锁定提示
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.skillData.cost, node.neededNodes, node.conflictNodes);
        // 锁定时显示锁定文案；否则显示正常需求

        skillRequirements.text = requirements;
    }

    public void LockedSkillEffect()
    {
        if (textEffectCo != null)
            StopCoroutine(textEffectCo);//避免重复启动导致闪烁冲突

        textEffectCo = StartCoroutine(TextBlinkEffectCo(skillRequirements, .15f, 3));//启动锁定提示闪烁
    }

    private IEnumerator TextBlinkEffectCo(TextMeshProUGUI text, float blinkInterval, int blinkCount)//锁定文本闪烁效果
    {
        for (int i = 0; i < blinkCount; i++)
        {
            text.text = GetColoredText(notmetConditionHex, lockedSkillText);//切换为“不满足条件”颜色
            yield return new WaitForSeconds(blinkInterval);//等待一次闪烁间隔

            text.text = GetColoredText(importantInfoHex, lockedSkillText);//切回重要提示颜色
            yield return new WaitForSeconds(blinkInterval);//等待一次闪烁间隔
        }
    }

    private string GetRequirements(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("技能需求:");

        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionHex : notmetConditionHex;//技能点够不够决定颜色
        string costText = $"- {skillCost} 技能点数";//技能点需求文本
        string finalCostText = GetColoredText(costColor, costText);//包装为带颜色文本

        sb.AppendLine(finalCostText);//写入技能点需求

        foreach (var node in neededNodes)
        {
            string nodeColor = node.isUnlocked ? metConditionHex : notmetConditionHex;//前置技能是否解锁决定颜色
            string nodeText = $"- {node.skillData.displayName}";//前置技能需求文本
            string finalNodeText = GetColoredText(nodeColor, nodeText);//包装为带颜色文本

            sb.AppendLine(finalNodeText);//写入前置技能需求
        }

        if (conflictNodes.Length <= 0)
            return sb.ToString();

        sb.AppendLine();
        sb.AppendLine(GetColoredText(importantInfoHex, "冲突技能: "));//写入冲突技能标题

        foreach (var node in conflictNodes)
        {
            string nodeText = $"- {node.skillData.displayName}";//冲突技能名称
            string finalNodeText = GetColoredText(importantInfoHex, nodeText);
            sb.AppendLine(finalNodeText);//写入冲突技能项
        }

        return sb.ToString();
    }

}
