using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_SkillToolTip skillToolTip;//技能提示框
    public UI_SkillTree skillTree;//技能树面板
    private bool skillTreeEnabled;

    private void Awake()
    {
        skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        skillTree = GetComponentInChildren<UI_SkillTree>(true);//在子节点中查找技能树（包含未激活对象）
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled;//切换技能树显示状态
        skillTree.gameObject.SetActive(skillTreeEnabled);//按当前状态显示/隐藏技能树
        skillToolTip.ShowToolTip(false, null);//切换面板时强制隐藏提示框，避免残留
    }
}
