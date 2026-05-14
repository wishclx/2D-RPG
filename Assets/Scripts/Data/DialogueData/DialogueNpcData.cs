using System;
using UnityEngine;

[Serializable]
public class DialogueNpcData
{
    public RewardType npcRewardType;//NPC奖励类型
    public QuestDataSO[] quests;

    public DialogueNpcData(RewardType npcRewardType, QuestDataSO[] quests)
    {
        this.npcRewardType = npcRewardType;
        this.quests = quests;
    }
}
