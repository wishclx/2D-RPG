using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_SkillToolTip skillToolTip;//技能工具提示
    public UI_SkillTree skillTree;//技能树
    private bool skillTreeEnabled;

    private void Awake()
    {
        skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        skillTree = GetComponentInChildren<UI_SkillTree>(true);//在UI对象的子对象中查找UI_SkillTree组件，即使它被禁用了
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled;//切换技能树的显示状态
        skillTree.gameObject.SetActive(skillTreeEnabled);//根据切换后的状态设置技能树对象的激活状态
        skillToolTip.ShowToolTip(false, null);//在切换技能树UI时，隐藏技能工具提示，以免它在技能树UI关闭后仍然显示在屏幕上
    }
}
