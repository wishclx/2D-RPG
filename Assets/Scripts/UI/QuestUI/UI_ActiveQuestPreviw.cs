using TMPro;
using UnityEngine;

public class UI_ActiveQuestPreviw : MonoBehaviour
{
    private Player_QuestManager questManager;

    //参数
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI progress;
    [SerializeField] private UI_QuestRewardSlot[] questRewardSlots;

    public void SetupQuestPreviw(QuestData questData)
    {
        questManager = Player.instance.questManager;
        QuestDataSO questSO = questData.questDataSO;

        questName.text = questSO.name;
        description.text = questSO.description;

        progress.text = questSO.questGoal + " " + questManager.GetQuestProgress(questData) + "/" + questSO.requiredAmount;//显示当前进度

        foreach (var obj in questRewardSlots)
            obj.gameObject.SetActive(false);

        for (int i = 0; i < questSO.rewardItems.Length; i++)
        {
            //创建新的奖励物品实例，确保物品信息完整
            Inventory_Item rewardItem = new Inventory_Item(questSO.rewardItems[i].itemData);
            rewardItem.stackSize = questSO.rewardItems[i].stackSize;

            questRewardSlots[i].gameObject.SetActive(true);
            questRewardSlots[i].UpdateSlot(rewardItem);//显示奖励物品
        }
    }
}
