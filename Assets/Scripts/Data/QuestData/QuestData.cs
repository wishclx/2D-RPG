using System;

[Serializable]
public class QuestData
{
    public QuestDataSO questDataSO;
    public int currentAmount;//当前数量
    public bool canGetReward;//是否可以领取奖励

    public void AddQuestProgress(int amount = 1)
    {
        currentAmount += amount;//增加当前数量
        canGetReward = CanGetReward();
    }

    public bool CanGetReward() => currentAmount >= questDataSO.requiredAmount;//是否可以领取奖励

    public QuestData(QuestDataSO questSO)
    {
        this.questDataSO = questSO;
    }
}