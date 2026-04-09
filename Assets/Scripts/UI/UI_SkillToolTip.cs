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
    [SerializeField] private TextMeshProUGUI skillRequirements;//技能需求

    [Space]
    [SerializeField] private string metConditionHex;//满足条件的颜色
    [SerializeField] private string notmetConditionHex;//不满足条件的颜色
    [SerializeField] private string importantInfoHex;//冲突信息的颜色
    [SerializeField] private Color exampleColor;//示例颜色
    [SerializeField] private string lockedSkillText = "选择了另一分支,此技能已锁定";//锁定技能的提示文本

    private Coroutine textEffectCo;//文本效果的协程

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

    public void ShowToolTip(bool show, RectTransform targetRect, UI_TreeNode node)//重载方法，增加了技能数据参数
    {
        base.ShowToolTip(show, targetRect);//调用基类的方法显示工具提示

        if (show == false)
            return;

        skillName.text = node.skillData.displayName;//设置技能名称文本
        skillDescription.text = node.skillData.description;

        string skillLockedText = GetColoredText(importantInfoHex, lockedSkillText);//根据锁定信息的颜色设置锁定技能的提示文本
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.skillData.cost, node.neededNodes, node.conflictNodes);
        //如果技能被锁定了，那么就显示锁定技能的提示文本，否则显示技能需求文本

        skillRequirements.text = requirements;
    }

    public void LockedSkillEffect()
    {
        if (textEffectCo != null)
            StopCoroutine(textEffectCo);//如果文本效果的协程已经在运行了，那么就停止它，以免多个协程同时运行导致文本闪烁效果混乱

        textEffectCo = StartCoroutine(TextBlinkEffectCo(skillRequirements, .15f, 3));//启动文本闪烁效果的协程，传入技能需求文本组件、闪烁间隔和闪烁次数
    }

    private IEnumerator TextBlinkEffectCo(TextMeshProUGUI text, float blinkInterval, int blinkCount)//文本闪烁效果的协程
    {
        for (int i = 0; i < blinkCount; i++)
        {
            text.text = GetColoredText(notmetConditionHex, lockedSkillText);//将文本设置为不满足条件的颜色和锁定技能的提示文本
            yield return new WaitForSeconds(blinkInterval);//等待一段时间

            text.text = GetColoredText(importantInfoHex, lockedSkillText);//将文本设置为满足条件的颜色和锁定技能的提示文本
            yield return new WaitForSeconds(blinkInterval);//等待一段时间
        }
    }

    private string GetRequirements(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("技能需求:");

        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionHex : notmetConditionHex;//根据技能点数是否足够来选择颜色
        string costText = $"- {skillCost} 技能点数";//技能点数需求的文本
        string finalCostText = GetColoredText(costColor, costText);//将技能点数需求文本包装在颜色标签中，以便在UI中显示带有颜色的文本

        sb.AppendLine(finalCostText);//添加技能点数需求，并根据是否满足条件来设置颜色

        foreach (var node in neededNodes)
        {
            string nodeColor = node.isUnlocked ? metConditionHex : notmetConditionHex;//根据技能节点是否解锁来选择颜色
            string nodeText = $"- {node.skillData.displayName}";//技能节点需求的文本
            string finalNodeText = GetColoredText(nodeColor, nodeText);//将技能节点需求文本包装在颜色标签中，以便在UI中显示带有颜色的文本

            sb.AppendLine(finalNodeText);//添加技能节点需求，并根据是否满足条件来设置颜色
        }

        if (conflictNodes.Length <= 0)
            return sb.ToString();

        sb.AppendLine();
        sb.AppendLine(GetColoredText(importantInfoHex, "冲突技能: "));//添加冲突技能的标题，并使用锁定信息的颜色来突出显示

        foreach (var node in conflictNodes)
        {
            string nodeText = $"- {node.skillData.displayName}";//与当前技能冲突的技能节点的文本
            string finalNodeText = GetColoredText(importantInfoHex, nodeText);
            sb.AppendLine(finalNodeText);//添加与当前技能冲突的技能节点，并使用重要信息的颜色来突出显示
        }

        return sb.ToString();
    }

}
