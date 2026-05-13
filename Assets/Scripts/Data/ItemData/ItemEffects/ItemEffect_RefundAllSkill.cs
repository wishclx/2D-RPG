using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Refund all skill", fileName = "Item effect Data - Refund all skill")]
public class ItemEffect_RefundAllSkill : ItemEffect_DataSO
{
    public override void ExecuteEffect()
    {
        UI ui = FindFirstObjectByType<UI>();
        //UI_SkillTree skillTree = FindFirstObjectByType<UI_SkillTree>(FindObjectsInactive.Include);//包括非激活对象
        ui.skillTreeUI.RefundAllSkills();
    }
}
