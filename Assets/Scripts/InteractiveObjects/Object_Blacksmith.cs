using UnityEngine;

public class Object_Blacksmith : Object_NPC, IInteractable
{
    private Animator anim;
    private Inventory_Player inventory;
    private Inventory_Storage storage;

    protected override void Awake()
    {
        base.Awake();
        //切换到铁匠铺的场景
        storage = GetComponent<Inventory_Storage>();
        anim = GetComponentInChildren<Animator>();
        anim.SetBool("isBlacksmith", true);//设置铁匠铺的动画状态
    }

    public override void Interact()
    {
        base.Interact();
        Debug.Log("打开铁匠铺");
        ui.storageUI.SetupStorageUI(storage);//设置仓库UI显示玩家库存和铁匠铺仓库
        ui.craftUI.SetupCraftUI(storage);//设置制作UI显示铁匠铺的UI

        ui.OpenStorageUI(true);//显示仓库UI
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<Inventory_Player>();//获取玩家的Inventory_Player组件
        storage.SetInventory(inventory);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (ui == null)
            return;

        ui.HideAllToolTips();//离开时关闭所有提示
        ui.OpenStorageUI(false);//关闭仓库UI
    }
}
