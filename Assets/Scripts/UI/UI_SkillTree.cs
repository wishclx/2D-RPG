using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private int skillPoints;
    [SerializeField] private UI_TreeConnectHandle[] parentNodes;//父节点连接点



    private void Start()
    {
        UpdataAllConnections();//在技能树界面打开时更新所有连接线
    }


    [ContextMenu("返还所有技能点并重置技能树")]
    public void RefundAllSkills()//返还所有技能点并重置技能树
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();//获取技能树中所有的技能节点

        foreach (var node in skillNodes)
        {
            if (node.isUnlocked == false)
                continue;//如果技能节点没有被解锁了，那么就跳过这个技能节点，不需要返还技能点和重置状态
            node.Refund();//调用每个技能节点的Refund方法，返还技能点并重置技能节点的状态
        }
    }

    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;//检查技能点是否足够
    public void RemoveSkillPoints(int cost) => skillPoints = skillPoints - cost;//减少技能点
    public void AddSkillPoints(int points) => skillPoints = skillPoints + points;//增加技能点

    [ContextMenu("更新所有连接线")]
    public void UpdataAllConnections()
    {
        foreach (var parent in parentNodes)
        {
            parent.UpdateAllConnections();//更新所有父节点连接点的连接线
        }
    }
}
