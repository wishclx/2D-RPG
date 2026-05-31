using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UI : MonoBehaviour
{
    public static UI instance;


    [SerializeField] private GameObject[] uiElements;// 场景中所有UI元素的引用数组，允许在编辑器中设置。
    public bool alternativeInput { get; private set; }// 备用输入模式，允许玩家使用不同的按键布局。
    private PlayerInputSet input;

    #region UI Components
    public UI_SkillToolTip skillToolTip
    { get; private set; }
    public UI_ItemToolTip itemToolTip { get; private set; }
    public UI_StatToolTip statToolTip { get; private set; }
    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_Inventory inventoryUI { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_Craft craftUI { get; private set; }
    public UI_Merchant merchantUI { get; private set; }
    public UI_InGame inGameUI { get; private set; }
    public UI_Options optionsUI { get; private set; }
    public UI_DeathScreen deathScreenUI { get; private set; }
    public UI_FadeScreen fadeScreenUI { get; private set; }
    public UI_Quest questUI { get; private set; }
    public UI_Dialogue dialogueUI { get; private set; }
    #endregion

    private bool skillTreeEnabled;
    private bool inventoryEnabled;

    private void Awake()
    {
        instance = this;

        itemToolTip = GetComponentInChildren<UI_ItemToolTip>(true);
        skillToolTip = GetComponentInChildren<UI_SkillToolTip>(true);
        statToolTip = GetComponentInChildren<UI_StatToolTip>(true);

        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);//在Awake中获取技能树UI组件，允许获取未激活的对象
        inventoryUI = GetComponentInChildren<UI_Inventory>(true);
        storageUI = GetComponentInChildren<UI_Storage>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        merchantUI = GetComponentInChildren<UI_Merchant>(true);
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        optionsUI = GetComponentInChildren<UI_Options>(true);
        deathScreenUI = GetComponentInChildren<UI_DeathScreen>(true);
        fadeScreenUI = GetComponentInChildren<UI_FadeScreen>(true);
        questUI = GetComponentInChildren<UI_Quest>(true);
        dialogueUI = GetComponentInChildren<UI_Dialogue>(true);

        skillTreeEnabled = skillTreeUI.gameObject.activeSelf;//跟随UI_SkillTree的初始状态
        inventoryEnabled = inventoryUI.gameObject.activeSelf;
    }

    private void Start()
    {
        skillTreeUI.UnlockDefaultSkills();//在游戏开始时解锁技能树中的默认技能。
    }

    public void SetupControlsUI(PlayerInputSet inputSet)
    {
        input = inputSet;

        input.UI.SkillTreeUI.performed += ctx => ToggleSkillTreeUI();// 绑定技能树UI的切换方法到输入事件。
        input.UI.InventoryUI.performed += ctx => ToggleInventoryUI();// 绑定背包UI的切换方法到输入事件。

        input.UI.Alternativelnput.performed += ctx => alternativeInput = true;// 切换备用输入模式的状态。
        input.UI.Alternativelnput.canceled += ctx => alternativeInput = false;

        input.UI.OptionUI.performed += ctx =>
        {
            // 如果设置界面已经打开，则关闭回到游戏界面
            if (optionsUI.gameObject.activeSelf)
            {
                SwitchToInGameUI();
                return;
            }

            // 如果有其他 UI（非游戏内 HUD）处于打开状态，先切回游戏界面
            bool otherUIActive = uiElements.Any(e => e.activeSelf && e != inGameUI.gameObject);
            if (otherUIActive)
            {
                SwitchToInGameUI();
                return;
            }

            // 否则打开设置界面
            OpenOptionsUI();
        };

        input.UI.DialogueInteraction.performed += ctx =>
        {
            if (dialogueUI.gameObject.activeInHierarchy)// 如果对话UI处于激活状态，调用对话交互方法。
                dialogueUI.DialogueInteraction();// 绑定对话交互方法到输入事件，允许玩家在对话界面进行交互。
        };

        input.UI.DialogueNavigation.performed += ctx =>
        {
            int direction = Mathf.RoundToInt(ctx.ReadValue<float>());// 读取输入值并转换为整数，表示对话选项的导航方向。

            if (dialogueUI.gameObject.activeInHierarchy)// 如果对话UI处于激活状态，调用对话选项导航方法。
                dialogueUI.NavigateChoices(direction);// 绑定对话选项导航方法到输入事件，允许玩家在对话界面导航选择。
        };
    }

    public void OpenDeathScreenUI()
    {
        SwitchTo(deathScreenUI.gameObject);// 切换到死亡界面UI，通常在玩家死亡时调用。
        input.Disable();// 禁用所有输入，确保玩家无法在死亡界面进行任何操作。(针对键鼠)
    }

    public void OpenOptionsUI()
    {
        HideAllToolTips();
        StopPlayerControls(true);// 禁用玩家输入，防止在选项菜单打开时进行游戏操作。
        SwitchTo(optionsUI.gameObject);// 切换到选项UI，通常在玩家按下选项键时调用。

        SyncPause();
    }

    public void SwitchToInGameUI()
    {
        HideAllToolTips();
        StopPlayerControls(false);// 重新启用玩家输入，允许玩家继续游戏。
        SwitchTo(inGameUI.gameObject);// 切换到游戏内UI，通常在关闭菜单或其他UI元素时调用。

        skillTreeEnabled = false;//重置技能树UI状态
        inventoryEnabled = false;
        SyncPause();
    }

    private void SwitchTo(GameObject objectToSwitchOn)
    {
        foreach (var element in uiElements)
            element.gameObject.SetActive(false);// 关闭所有UI元素，确保只有目标UI元素被激活。

        objectToSwitchOn.SetActive(true);// 激活目标UI元素
    }

    private void StopPlayerControls(bool stopControls)
    {
        if (stopControls)
            input.Player.Disable();// 禁用玩家输入，通常在游戏暂停或玩家死亡时调用。
        else
            input.Player.Enable();
    }

    private void StopPlayerControlsIfNeeded()
    {
        foreach (var element in uiElements)
        {
            if (element.activeSelf)
            {
                StopPlayerControls(true);// 如果有任何UI元素处于激活状态，则禁用玩家输入。
                return;
            }
        }

        StopPlayerControls(false);// 否则，启用玩家输入。
    }

    private void SyncPause() => Time.timeScale =
        (skillTreeUI.gameObject.activeSelf
        || inventoryUI.gameObject.activeSelf
        || storageUI.gameObject.activeSelf
        || craftUI.gameObject.activeSelf
        || merchantUI.gameObject.activeSelf
        || optionsUI.gameObject.activeSelf) ? 0f : 1f;

    public void ToggleSkillTreeUI()
    {
        skillTreeUI.transform.SetAsLastSibling();//确保技能树工具提示在其他UI元素之上显示
        SetToolTipAsLastSibling();
        fadeScreenUI.transform.SetAsLastSibling();//确保淡入淡出屏幕在其他UI元素之上显示

        skillTreeEnabled = !skillTreeEnabled;//切换技能树UI的状态
        skillTreeUI.gameObject.SetActive(skillTreeEnabled);
        HideAllToolTips();

        StopPlayerControlsIfNeeded();
        SyncPause();
    }

    public void ToggleInventoryUI()
    {
        inventoryUI.transform.SetAsLastSibling();//确保背包UI在其他UI元素之上显示
        SetToolTipAsLastSibling();//确保工具提示在其他UI元素之上显示
        fadeScreenUI.transform.SetAsLastSibling();//确保淡入淡出屏幕在其他UI元素之上显示

        inventoryEnabled = !inventoryEnabled;
        inventoryUI.gameObject.SetActive(inventoryEnabled);
        HideAllToolTips();

        StopPlayerControlsIfNeeded();
        SyncPause();
    }

    public void OpenDialogueUI(DialogueLineSO firstLine, DialogueNpcData npcData)
    {
        StopPlayerControls(true);// 禁用玩家输入，防止在对话界面打开时进行游戏操作。
        HideAllToolTips();// 隐藏所有工具提示

        dialogueUI.gameObject.SetActive(true);// 激活对话UI，通常在与NPC交互时调用。
        dialogueUI.SetupNpcData(npcData);// 设置对话NPC的数据，允许对话UI显示正确的NPC信息和对话选项。
        dialogueUI.PlayDialogueLine(firstLine);
    }

    public void OpenQuestUI(QuestDataSO[] questToShow)
    {
        StopPlayerControls(true);
        HideAllToolTips();

        questUI.gameObject.SetActive(true);
        questUI.SetupQuestUI(questToShow);// 打开任务UI并传递要显示的任务数据。
    }

    public void OpenStorageUI(bool openStorageUI)
    {
        storageUI.gameObject.SetActive(openStorageUI);
        StopPlayerControls(openStorageUI);

        if (!openStorageUI)
        {
            craftUI.gameObject.SetActive(false);//关闭制作UI
            HideAllToolTips();
        }
        SyncPause();
    }

    public void OpenCraftUI(bool openStorageUI)
    {
        craftUI.gameObject.SetActive(openStorageUI);
        StopPlayerControls(openStorageUI);

        if (!openStorageUI)
        {
            storageUI.gameObject.SetActive(false);
            HideAllToolTips();
        }
        SyncPause();
    }

    public void OpenMerchantUI(bool openMerchantUI)
    {
        merchantUI.gameObject.SetActive(openMerchantUI);
        StopPlayerControls(openMerchantUI);

        if (!openMerchantUI)
            HideAllToolTips();
        SyncPause();
    }

    public void HideAllToolTips()
    {
        if (skillToolTip != null) skillToolTip.ShowToolTip(false, null);
        if (itemToolTip != null) itemToolTip.ShowToolTip(false, null);
        if (statToolTip != null) statToolTip.ShowToolTip(false, null);
    }

    private void SetToolTipAsLastSibling()// 确保工具提示在其他UI元素之上显示，通常在显示工具提示时调用。
    {
        itemToolTip.transform.SetAsLastSibling();
        skillToolTip.transform.SetAsLastSibling();
        statToolTip.transform.SetAsLastSibling();
    }
}
