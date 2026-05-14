using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : MonoBehaviour
{
    private UI ui;
    private DialogueNpcData npcData;
    private Player_QuestManager questManager;

    [SerializeField] private Image speakerPortrait;//头像
    [SerializeField] private TextMeshProUGUI speakerName;//名字
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI[] dialogueChoicesText;

    [Space]
    [SerializeField] private float textSpeed = .1f;
    private string fullTextToShow;
    private Coroutine typeTextCo;

    private DialogueLineSO currentLine;
    private DialogueLineSO[] currentChoices;//当前对话选项行数据
    private DialogueLineSO selectedChoice;
    private int selectedChoiceIndex;

    private bool waitingToConfirm;
    private bool canInteract;//是否可以进行对话交互（如点击继续、选择对话选项等）

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        questManager = Player.instance.questManager;
    }

    public void SetupNpcData(DialogueNpcData npcData)
    {
        this.npcData = npcData;
    }

    public void PlayDialogueLine(DialogueLineSO line)
    {
        currentLine = line;//记录当前对话行，用于后续的对话交互
        currentChoices = line.choiceLines;//记录当前对话选项行数据，用于显示对话选项
        canInteract = false;//在开始播放对话行时，暂时禁止对话交互，直到文本完全显示或玩家点击继续
        selectedChoice = null;//重置已选择的选项
        selectedChoiceIndex = 0;//重置选项索引

        HideAllChoices();

        speakerPortrait.sprite = line.speaker.speakerPortrait;
        speakerName.text = line.speaker.speakerName;

        //如果对话行没有特定的动作类型，或者动作类型是玩家选择，则显示随机文本；否则显示预设的动作文本
        fullTextToShow = line.actionType == DialogueActionType.None || line.actionType == DialogueActionType.PlayerMakeChoice ?
            line.GetRandomLine() : line.actionLine;

        typeTextCo = StartCoroutine(TypeTextCo(fullTextToShow));//逐字显示文本
        StartCoroutine(EnableInteractionCo());//在文本完全显示后，允许对话交互)
    }

    private void HandleNextAction()
    {
        switch (currentLine.actionType)
        {
            case DialogueActionType.OpenShop:
                ui.SwitchToInGameUI();
                ui.OpenMerchantUI(true);
                break;
            case DialogueActionType.PlayerMakeChoice:
                if (selectedChoice == null)
                {
                    ShowChoices();//显示对话选项UI
                    waitingToConfirm = true;//允许确认选项
                }
                else
                {
                    selectedChoice = currentChoices[selectedChoiceIndex];//获取玩家选择的对话选项行数据
                    PlayDialogueLine(selectedChoice);
                    selectedChoice = null;//重置已选择的选项，准备下一次对话交互
                }
                break;
            case DialogueActionType.OpenQuest:
                ui.SwitchToInGameUI();
                ui.OpenQuestUI(npcData.quests);
                break;
            case DialogueActionType.GetQuestReward:
                ui.SwitchToInGameUI();
                questManager.TryGetRewardFrom(npcData.npcRewardType);//尝试从NPC处获取任务奖励
                break;
            case DialogueActionType.OpenCraft:
                ui.SwitchToInGameUI();
                ui.OpenCraftUI(true);
                break;
            case DialogueActionType.CloseDialogue:
                ui.SwitchToInGameUI();
                break;
        }
    }

    public void DialogueInteraction()
    {
        if (canInteract == false)
            return;

        if (typeTextCo != null)
        {
            CompleteTyping();//如果正在逐字显示文本，直接显示完整文本

            if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
                waitingToConfirm = true;//文本完全显示后，设置等待确认状态，等待玩家点击继续
            else
                HandleNextAction();//如果当前对话行是玩家选择类型，文本显示完成后直接处理后续动作（如显示对话选项等）

            return;
        }

        if (waitingToConfirm)
        {
            waitingToConfirm = false;//重置等待确认状态
            HandleNextAction();//处理对话行的后续动作（如打开商店界面等）
        }
    }

    private void CompleteTyping()
    {
        if (typeTextCo != null)
        {
            StopCoroutine(typeTextCo);//停止逐字显示文本的协程
            dialogueText.text = fullTextToShow;//直接显示完整文本
            typeTextCo = null;
        }
    }

    private void ShowChoices()
    {
        for (int i = 0; i < dialogueChoicesText.Length; i++)
        {
            if (i < currentChoices.Length)
            {
                DialogueLineSO choice = currentChoices[i];//获取当前对话选项行数据
                string choiceText = choice.playerChoiceAnswer;//获取对话选项文本内容

                dialogueChoicesText[i].gameObject.SetActive(true);//显示当前对话选项UI元素
                dialogueChoicesText[i].text = selectedChoiceIndex == i ?
                    $"<color=yellow> {i + 1}){choiceText}</color>" :
                    $"{i + 1}){choiceText}";//突出显示当前选择的选项

                if (choice.actionType == DialogueActionType.GetQuestReward
                    && questManager.HasRewardAvailableFor(npcData.npcRewardType) == false)
                    //如果当前NPC没有可领取奖励，则隐藏领取奖励选项
                    dialogueChoicesText[i].gameObject.SetActive(false);
            }
            else
            {
                dialogueChoicesText[i].gameObject.SetActive(false);//隐藏多余的对话选项UI元素
            }
        }

        selectedChoice = currentChoices[selectedChoiceIndex];//更新当前选择的对话选项行数据，准备后续的对话交互
    }

    private void HideAllChoices()
    {
        foreach (var obj in dialogueChoicesText)
            obj.gameObject.SetActive(false);//隐藏所有对话选项UI元素
    }

    public void NavigateChoices(int direction)
    {
        if (currentChoices == null || currentChoices.Length <= 1)
            return;

        selectedChoiceIndex += direction;
        selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, currentChoices.Length - 1);//确保选择索引在有效范围内
        ShowChoices();//更新对话选项UI显示，突出显示当前选择的选项
    }

    private IEnumerator TypeTextCo(string text)
    {
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
        {
            waitingToConfirm = true;//文本完全显示后，设置等待确认状态，等待玩家点击继续
        }
        else
        {
            yield return new WaitForSeconds(.2f);
            selectedChoice = null;
            HandleNextAction();//如果当前对话行是玩家选择类型，文本显示完成后直接处理后续动作（如显示对话选项等）
        }

        typeTextCo = null;
    }

    private IEnumerator EnableInteractionCo()
    {
        yield return null;
        canInteract = true;
    }
}
