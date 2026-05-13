using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ActiveQuestSlot : MonoBehaviour
{
    private QuestData questInSlot;
    private UI_ActiveQuestPreviw questPreviw;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image[] questRewardPreviw;

    public void SetupActiveQuestSlot(QuestData questToSetup)
    {
        //设置预览界面
        questPreviw = transform.root.GetComponentInChildren<UI_ActiveQuestPreviw>();
        questInSlot = questToSetup;

        questName.text = questToSetup.questDataSO.questName;

        Inventory_Item[] reward = questToSetup.questDataSO.rewardItems;//显示奖励预览

        foreach (var previwIcon in questRewardPreviw)
        {
            previwIcon.gameObject.SetActive(false);//先隐藏所有预览图标
        }

        for (int i = 0; i < reward.Length; i++)
        {
            if (reward[i] == null) continue;
            Image previw = questRewardPreviw[i];//显示预览图标

            previw.gameObject.SetActive(true);
            previw.sprite = reward[i].itemData.itemIcon;
            previw.GetComponentInChildren<TextMeshProUGUI>().text = reward[i].stackSize.ToString();//显示数量
        }
    }

    public void SetupPreviwBTN()
    {
        questPreviw.SetupQuestPreviw(questInSlot);//设置预览界面内容
    }
}
