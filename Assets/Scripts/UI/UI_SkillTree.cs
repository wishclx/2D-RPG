using UnityEngine;

/// <summary>
/// UI_SkillTree 的职责说明。
/// </summary>
public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private int skillPoints;
    [SerializeField] private UI_TreeConnectHandle[] parentNodes;//技能树中所有父节点的连接句柄。
    public Player_SkillManager skillManager { get; private set; }


    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    private void Awake()
    {
        skillManager = FindAnyObjectByType<Player_SkillManager>();
    }

    /// <summary>
    /// 执行 Start 逻辑。
    /// </summary>
    private void Start()
    {
        UpdataAllConnections();
    }


    [ContextMenu("重置技能树")]//
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
        {
            if (node.isUnlocked)
            {
                node.Refund();
                continue;
            }

            node.isUnlocked = false;
            node.isLocked = false;
        }

        UpdataAllConnections();
    }

    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;
    public void RemoveSkillPoints(int cost) => skillPoints = skillPoints - cost;
    public void AddSkillPoints(int points) => skillPoints = skillPoints + points;

    [ContextMenu("更新所有连接")]
    /// <summary>
    /// 执行 UpdataAllConnections 逻辑。
    /// </summary>
    public void UpdataAllConnections()
    {
        foreach (var parent in parentNodes)
        {
            parent.UpdateAllConnections();
        }
    }
}


