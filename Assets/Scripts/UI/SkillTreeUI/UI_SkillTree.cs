using System.Linq;
using TMPro;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour, ISaveable
{
    [SerializeField] private int skillPoints;
    [SerializeField] private TextMeshProUGUI skillPointsText;//显示当前技能点数量的UI文本组件。
    [SerializeField] private UI_TreeConnectHandle[] parentNodes;//技能树中所有父节点的连接句柄。
    private UI_TreeNode[] allTreeNodes;
    public Player_SkillManager skillManager { get; private set; }

    private void Start()
    {
        UpdataAllConnections();
        UpdateSkillPointsUI();
    }

    private void UpdateSkillPointsUI()
    {
        skillPointsText.text = skillPoints.ToString();//在游戏开始时更新技能点数量的显示。
    }

    public void UnlockDefaultSkills()
    {
        allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);
        skillManager = FindAnyObjectByType<Player_SkillManager>();

        foreach (var node in allTreeNodes)
            node.UnlockDefaultSkill();//解锁默认技能。
    }


    [ContextMenu("重置技能树")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>(true);// 包含未激活节点

        skillManager.ResetAllSkillUpgrades();// 先把所有技能升级类型清空，并恢复技能槽默认外观

        foreach (var node in skillNodes)
        {
            node.isLocked = false;// 清除“冲突锁定”状态
            node.Refund();// 退还非默认已解锁节点
        }

        // 重新处理默认技能（会自动遵守互斥规则）
        UnlockDefaultSkills();

        // 重新应用当前仍为已解锁的节点
        foreach (var node in skillNodes)
        {
            if (node.isUnlocked)
                skillManager.GetSkillByType(node.skillData.skillType).SetSkillUpgrade(node.skillData);
        }

        UpdataAllConnections();
    }

    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;

    public void RemoveSkillPoints(int cost)
    {
        skillPoints = skillPoints - cost;
        UpdateSkillPointsUI();
    }

    public void AddSkillPoints(int points)
    {
        skillPoints = skillPoints + points;
        UpdateSkillPointsUI();
    }

    [ContextMenu("更新所有连接")]
    public void UpdataAllConnections()
    {
        foreach (var parent in parentNodes)
        {
            parent.UpdateAllConnections();
        }
    }

    public void LoadData(GameData data)
    {
        skillPoints = data.skillPoints;

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.displayName;// 从保存的数据中获取技能节点的解锁状态，并根据状态解锁技能。

            if (data.skillTreeUI.TryGetValue(skillName, out bool unlocked) && unlocked)// 如果技能节点在保存的数据中存在且被标记为已解锁，则解锁该技能节点。
                node.UnlockWithSaveData();
        }

        foreach (var skill in skillManager.allSkills)
        {
            if (data.skillUpgrades.TryGetValue(skill.GetSkillType(), out SkillUpgradeType upgradeType))// 从保存的数据中获取技能的升级类型，并根据类型设置技能状态。
            {
                // 根据技能类型在保存的数据中找到对应的升级类型，并在技能管理器中找到对应的技能实例。
                var upgradeData = allTreeNodes.FirstOrDefault(node => node.skillData.upgradeData.upgradeType == upgradeType);

                if (upgradeData != null)
                    skill.SetSkillUpgrade(upgradeData.skillData);// 根据保存的数据设置技能的升级状态。   
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.skillPoints = skillPoints;
        data.skillTreeUI.Clear();
        data.skillUpgrades.Clear();

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.displayName;
            data.skillTreeUI[skillName] = node.isUnlocked;
        }

        foreach (var skill in skillManager.allSkills)
        {
            data.skillUpgrades[skill.GetSkillType()] = skill.GetUpgradeType();// 保存每个技能的升级类型，以便在加载时恢复技能状态。
        }
    }
}


