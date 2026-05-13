using UnityEngine;

public class UI_Quest : MonoBehaviour, ISaveable
{
    private GameData currentGameData;//当前游戏数据

    [SerializeField] private UI_ItemSlotParent inventorySlots;
    [SerializeField] private UI_QuestPreviw questPreviw;

    private UI_QuestSlot[] questSlots;//任务槽数组
    public Player_QuestManager questManager { get; private set; }//玩家任务管理器

    private void Awake()
    {
        questSlots = GetComponentsInChildren<UI_QuestSlot>(true);
        questManager = Player.instance.questManager;
    }

    public void SetupQuestUI(QuestDataSO[] questToSetup)
    {
        foreach (var slot in questSlots)
            slot.gameObject.SetActive(false);

        for (int i = 0; i < questToSetup.Length; i++)
        {
            questSlots[i].gameObject.SetActive(true);
            questSlots[i].SetupQuestSlot(questToSetup[i]);//设置每个任务槽的任务数据
        }

        questPreviw.MakeQuestPreviwEmpty();
        inventorySlots.UpdateSlots(Player.instance.inventory.itemList);//更新任务UI中的物品槽显示，显示玩家当前的物品

        UpdateQuestList();
    }

    public void UpdateQuestList()
    {
        foreach (var slot in questSlots)
        {
            if (slot.questInSlot == null) continue;

            if (slot.gameObject.activeSelf && CanTakeQuest(slot.questInSlot) == false)//任务不可接且当前显示时，隐藏该任务槽
                slot.gameObject.SetActive(false);
        }
    }

    private bool CanTakeQuest(QuestDataSO questToCheck)
    {
        bool questActive = questManager.QuestIsActive(questToCheck);

        if (currentGameData != null)
        {
            //检查当前游戏数据中是否已经完成了该任务
            bool questCompleted =
                currentGameData.completedQuests.TryGetValue(questToCheck.questSaveId, out bool iscompleted) && iscompleted;

            return questActive == false && questCompleted == false;//如果任务未激活且未完成，则可以接取该任务
        }

        return questActive == false;
    }

    public UI_QuestPreviw GetQuestPreviw() => questPreviw;

    public void LoadData(GameData data)
    {
        currentGameData = data;
    }

    public void SaveData(ref GameData data)
    {

    }
}
