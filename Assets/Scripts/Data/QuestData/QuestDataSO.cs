using UnityEditor;
using UnityEngine;

public enum RewardType { Merchant, Blacksmith, None }
public enum QuestType { Kill, Talk, Delivery }

[CreateAssetMenu(menuName = "RPG Setup/ Quest Data/New Quest", fileName = "任务 -")]
public class QuestDataSO : ScriptableObject
{
    public string questSaveId;//保存ID，唯一标识一个任务数据，可以用来在保存和加载时识别任务
    [Space]
    public QuestType questType;
    public string questName;
    [TextArea] public string description;
    [TextArea] public string questGoal;//任务目标描述

    public string questTargetId;//任务目标ID，可以用来在保存和加载时识别任务目标，比如击败某个敌人，收集某个物品等
    public int requiredAmount;//完成任务目标所需的数量，比如击败10个敌人，收集5个物品等
    public ItemDataSO itemToDeliver;//如果任务类型是交付任务，则需要交付的物品数据

    [Header("奖励")]
    public RewardType rewardType;
    public Inventory_Item[] rewardItems;//奖励物品，可以是商店物品或者铁匠物品，根据rewardType来区分

    private void OnValidate()
    {

#if UNITY_EDITOR
        string path = UnityEditor.AssetDatabase.GetAssetPath(this);
        questSaveId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
}
