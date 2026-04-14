using UnityEngine;

/// <summary>
/// UI 的职责说明。
/// </summary>
public class UI : MonoBehaviour
{
    public UI_SkillToolTip skillToolTip;
    public UI_SkillTree skillTree;
    private bool skillTreeEnabled;

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    private void Awake()
    {
        skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        skillTree = GetComponentInChildren<UI_SkillTree>(true);
    }

    /// <summary>
    /// 执行 ToggleSkillTreeUI 逻辑。
    /// </summary>
    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled;
        skillTree.gameObject.SetActive(skillTreeEnabled);
        skillToolTip.ShowToolTip(false, null);
    }
}


