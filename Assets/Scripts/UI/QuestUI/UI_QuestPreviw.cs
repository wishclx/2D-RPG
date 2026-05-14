using TMPro;
using UnityEngine;

public class UI_QuestPreviw : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI questGoal;
    [SerializeField] private UI_QuestRewardSlot[] questReward;//任务奖励

    [SerializeField] private GameObject[] additionalObject;//额外的UI对象，如完成标志等
    private UI_Quest questUI;//任务UI组件
    private QuestDataSO previwQuest;//当前预览的任务数据

    public void SetupQuestPreviw(QuestDataSO questDataSO)
    {
        questUI = transform.root.GetComponentInChildren<UI_Quest>();
        previwQuest = questDataSO;

        EnableAdditionalObject(true);//显示额外UI对象  
        EnableQuestRewardObject(false);//先隐藏所有奖励槽

        questName.text = questDataSO.name;
        questDescription.text = questDataSO.description;

        if (questDataSO.questType == QuestType.Talk)
            questGoal.text = questDataSO.questGoal;//对话类任务不显示数量
        else
            questGoal.text = questDataSO.questGoal + " " + questDataSO.requiredAmount;//其他任务显示数量


        //根据任务数据设置奖励槽
        for (int i = 0; i < questDataSO.rewardItems.Length; i++)
        {
            //创建一个新的Inventory_Item实例，确保每个奖励槽都有独立的物品数据
            Inventory_Item rewardItem = new Inventory_Item(questDataSO.rewardItems[i].itemData);
            rewardItem.stackSize = questDataSO.rewardItems[i].stackSize;

            questReward[i].gameObject.SetActive(true);
            questReward[i].UpdateSlot(rewardItem);
        }
    }

    public void AcceptQuestBTN()
    {
        MakeQuestPreviwEmpty();

        questUI.questManager.AcceptQuest(previwQuest);//接受任务
        questUI.UpdateQuestList();
    }

    public void MakeQuestPreviwEmpty()
    {
        questName.text = "";
        questDescription.text = "";

        EnableAdditionalObject(false);//隐藏所有额外UI对象
        EnableQuestRewardObject(false);
    }

    private void EnableAdditionalObject(bool enable)
    {
        foreach (var obj in additionalObject)
            obj.SetActive(enable);
    }

    private void EnableQuestRewardObject(bool enable)
    {
        foreach (var obj in questReward)
            obj.gameObject.SetActive(enable);
    }
}
