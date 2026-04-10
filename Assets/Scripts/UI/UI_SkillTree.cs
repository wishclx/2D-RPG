using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private int skillPoints;
    [SerializeField] private UI_TreeConnectHandle[] parentNodes;//父节点连接句柄
    public Player_SkillManager skillManager { get; private set; }


    private void Awake()
    {
        skillManager = FindAnyObjectByType<Player_SkillManager>();
    }

    private void Start()
    {
        UpdataAllConnections();//初始化时刷新全部连接线
    }


    [ContextMenu("返还所有技能点并重置技能树")]
    public void RefundAllSkills()//重置技能树状态并按规则返还技能点
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();//获取技能树中的全部技能节点

        foreach (var node in skillNodes)
        {
            if (node.isUnlocked)
            {
                node.Refund();//已解锁：返还技能点并重置
                continue;
            }

            node.isUnlocked = false;//未解锁：仅重置状态，不返还技能点
            node.isLocked = false;
        }

        UpdataAllConnections();//刷新连接线显示
    }

    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;//技能点是否足够
    public void RemoveSkillPoints(int cost) => skillPoints = skillPoints - cost;//扣除技能点
    public void AddSkillPoints(int points) => skillPoints = skillPoints + points;//增加技能点

    [ContextMenu("更新所有连接线")]
    public void UpdataAllConnections()
    {
        foreach (var parent in parentNodes)
        {
            parent.UpdateAllConnections();//从父节点开始递归刷新连接线
        }
    }
}
