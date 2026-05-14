using System.Collections.Generic;
using UnityEngine;

public class Player_QuestManager : MonoBehaviour, ISaveable
{
    public List<QuestData> activeQuests;
    public List<QuestData> completedQuests;

    private Entity_DropManager dropManager;
    private Inventory_Player inventory;

    [Header("任务数据库")]
    [SerializeField] private QuestDatabaseSO questDatabase;

    private void Awake()
    {
        dropManager = GetComponent<Entity_DropManager>();
        inventory = GetComponent<Inventory_Player>();
    }

    public void TryGetRewardFrom(RewardType npcType)
    {
        List<QuestData> getRewardQuests = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            //检查每个活动任务，如果任务类型是交付任务，检查玩家是否有足够的物品来完成任务进度
            if (quest.questDataSO.questType == QuestType.Delivery)//如果任务类型是交付任务，获取所需的物品和数量
            {
                var requiredItem = quest.questDataSO.itemToDeliver;
                var requiredAmount = quest.questDataSO.requiredAmount;

                if (inventory.HasItemAmount(requiredItem, requiredAmount))
                {
                    inventory.RemoveItemAmount(requiredItem, requiredAmount);//如果玩家有足够的物品，移除相应数量
                    quest.AddQuestProgress(requiredAmount);//为任务增加相应的进度
                }
            }

            if (quest.CanGetReward() && quest.questDataSO.rewardType == npcType)
                getRewardQuests.Add(quest);//如果任务可以领取奖励且奖励类型匹配，则添加到待领取列表
        }

        foreach (var quest in getRewardQuests)
        {
            GiveQuestReward(quest.questDataSO);//为每个待领取的任务发放奖励
            CompleteQuest(quest);//完成任务
        }
    }

    private void GiveQuestReward(QuestDataSO questDataSO)
    {
        foreach (var item in questDataSO.rewardItems)
        {
            if (item == null || item.itemData == null) continue;//跳过无效的奖励项

            for (int i = 0; i < item.stackSize; i++)
            {
                dropManager.CreateItemDrop(item.itemData);//在玩家位置生成奖励物品
            }
        }
    }

    public bool HasCompletedQuest()
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            QuestData quest = activeQuests[i];
            if (quest.questDataSO.questType == QuestType.Delivery)//如果任务类型是交付任务，获取所需的物品和数量
            {
                var requiredItem = quest.questDataSO.itemToDeliver;
                var requiredAmount = quest.questDataSO.requiredAmount;

                if (inventory.HasItemAmount(requiredItem, requiredAmount))
                    return true;
            }

            if (quest.CanGetReward())
                return true;
        }
        return false;
    }

    public void AddProgress(string questTargetId, int amout = 1)
    {
        List<QuestData> getRewardQuests = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            if (quest.questDataSO.questTargetId != questTargetId)
                continue;//跳过不匹配的任务目标ID

            if (quest.CanGetReward() == false)
                quest.AddQuestProgress(amout);//为匹配的任务增加进度

            if (quest.questDataSO.rewardType == RewardType.None && quest.CanGetReward())
                getRewardQuests.Add(quest);//如果任务奖励可以直接领取，则添加到待领取列表
        }

        foreach (var quest in getRewardQuests)
        {
            GiveQuestReward(quest.questDataSO);//为每个待领取的任务发放奖励
            CompleteQuest(quest);//完成任务
        }
    }

    public int GetQuestProgress(QuestData questToCheck)
    {
        QuestData quest = activeQuests.Find(q => q == questToCheck);//在活动任务列表中查找匹配的任务数据

        return quest != null ? quest.currentAmount : 0;//如果找到匹配的任务数据，返回当前进度；否则返回0
    }

    public void AcceptQuest(QuestDataSO questDataSO)
    {
        activeQuests.Add(new QuestData(questDataSO));
    }

    public void CompleteQuest(QuestData questData)
    {
        completedQuests.Add(questData);
        activeQuests.Remove(questData);
    }

    public bool QuestIsActive(QuestDataSO questToCheck)
    {
        if (questToCheck == null)
            return false;

        return activeQuests.Find(q => q.questDataSO == questToCheck) != null;
    }

    public void LoadData(GameData data)
    {
        activeQuests.Clear();//清空现有的活动任务列表

        foreach (var entry in data.activeQuests)
        {
            string questSaveId = entry.Key;
            int progress = entry.Value;

            QuestDataSO questDataSO = questDatabase.GetQuestById(questSaveId);//从任务数据库中获取对应保存ID的任务数据

            if (questDataSO == null)
            {
                Debug.LogWarning($"无法找到保存ID为 {questSaveId} 的任务数据。请确保该任务数据存在于数据库中。");
                continue;//如果找不到对应的任务数据，跳过该条目
            }

            QuestData questToLoad = new QuestData(questDataSO);//创建新的任务数据实例
            questToLoad.currentAmount = progress;

            activeQuests.Add(questToLoad);//将加载的任务添加到活动任务列表中
        }


    }

    public void SaveData(ref GameData data)
    {
        data.activeQuests.Clear();//保存前清空进行中任务，避免重复键
        data.completedQuests.Clear();//保存前清空已完成任务，避免重复键

        foreach (var quest in activeQuests)
        {
            string questSaveId = quest.questDataSO.questSaveId;
            if (data.activeQuests.ContainsKey(questSaveId))
            {
                data.activeQuests[questSaveId] = quest.currentAmount;//已存在则覆盖进度
                continue;
            }

            data.activeQuests.Add(questSaveId, quest.currentAmount);//添加进行中任务
        }

        foreach (var quest in completedQuests)
        {
            string questSaveId = quest.questDataSO.questSaveId;
            if (data.completedQuests.ContainsKey(questSaveId))
            {
                data.completedQuests[questSaveId] = true;//已存在则覆盖完成状态
                continue;
            }

            data.completedQuests.Add(questSaveId, true);//添加已完成任务
        }
    }

    public bool HasRewardAvailableFor(RewardType npcType)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questDataSO.rewardType != npcType)
                continue;//只检查当前NPC的任务

            if (quest.questDataSO.questType == QuestType.Delivery)
            {
                var requiredItem = quest.questDataSO.itemToDeliver;
                var requiredAmount = quest.questDataSO.requiredAmount;

                if (requiredItem == null || requiredAmount <= 0)
                    continue;//交付任务数据异常，跳过

                if (inventory.HasItemAmount(requiredItem, requiredAmount))
                    return true;//材料足够，可领奖
            }
            else
            {
                if (quest.CanGetReward())
                    return true;//非交付任务满足条件，可领奖
            }
        }

        return false;
    }
}
