using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image[] rewardQuickPreviewSlots;

    public QuestDataSO questInSlot { get; private set; }
    private UI_QuestPreviw questPreviw;

    public void SetupQuestSlot(QuestDataSO questDataSO)
    {
        questPreviw = transform.root.GetComponentInChildren<UI_Quest>().GetQuestPreviw();

        questInSlot = questDataSO;
        questName.text = questDataSO.questName;

        //设置奖励预览图标
        foreach (var previwIcon in rewardQuickPreviewSlots)
        {
            previwIcon.gameObject.SetActive(false);
        }

        for (int i = 0; i < questInSlot.rewardItems.Length; i++)
        {
            //如果奖励物品数据为空，则跳过
            if (questDataSO.rewardItems[i] == null || questDataSO.rewardItems[i].itemData == null)
                continue;

            Image slot = rewardQuickPreviewSlots[i];//获取预览槽位

            slot.gameObject.SetActive(true);
            slot.sprite = questDataSO.rewardItems[i].itemData.itemIcon;//设置预览图标
            slot.GetComponentInChildren<TextMeshProUGUI>().text = questDataSO.rewardItems[i].stackSize.ToString();//设置预览数量
        }
    }

    public void UpdateQuestPreviw()
    {
        questPreviw.SetupQuestPreviw(questInSlot);//更新任务预览
    }
}
