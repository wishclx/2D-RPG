using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_ActiveQuest : MonoBehaviour
{
    private Player_QuestManager questManager;
    private UI_ActiveQuestSlot[] questSlots;

    private void Awake()
    {
        questManager = Player.instance.questManager;
        questSlots = GetComponentsInChildren<UI_ActiveQuestSlot>(true);
    }

    private void OnEnable()
    {
        List<QuestData> quests = questManager.activeQuests;//获取当前玩家的所有活跃任务

        foreach (var slot in questSlots)
        {
            slot.gameObject.SetActive(false);//先将所有任务槽隐藏
        }

        for (int i = 0; i < quests.Count; i++)
        {
            questSlots[i].gameObject.SetActive(true);
            questSlots[i].SetupActiveQuestSlot(quests[i]);//将每个任务槽设置为对应的活跃任务
        }

        if (quests.Count > 0)
            questSlots[0].SetupPreviwBTN();//设置第一个任务槽的预览显示
    }
}
