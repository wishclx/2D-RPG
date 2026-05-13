using UnityEngine;

public class Object_Merchant : Object_NPC, IInteractable
{
    [Header("任务与对白")]
    [SerializeField] private QuestDataSO[] quests;//商人提供的任务数据数组

    private Inventory_Player inventory;
    private Inventory_Merchant merchant;

    protected override void Awake()
    {
        base.Awake();
        merchant = GetComponent<Inventory_Merchant>();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Z))
            merchant.FillShopList();//按下Z键刷新商店物品列表
    }

    public override void Interact()
    {
        base.Interact();

        ui.OpenQuestUI(quests);//显示商人提供的任务UI，传入商人提供的任务数据数组

        //ui.merchantUI.SetupMerchantUI(merchant, inventory);//设置商店UI，传入商店和玩家的物品栏
        //ui.OpenMerchantUI(true);//显示商店UI
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<Inventory_Player>();
        merchant.SetInventory(inventory);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (ui == null)
            return;

        ui.HideAllToolTips();
        ui.OpenMerchantUI(false);
    }

}
