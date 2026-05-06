using UnityEngine;

public class Object_ItemPickup : MonoBehaviour
{

    [SerializeField] private Vector2 dropForce = new Vector2(3, 10);//物品掉落时的初始力，可以在编辑器中调整
    [SerializeField] private ItemDataSO itemData;//从ItemDataSO中获取物品数据

    [Space]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;

    private void OnValidate()
    {
        if (itemData == null)
            return;

        sr = GetComponent<SpriteRenderer>();
        SetupVisuals();
    }

    public void SetupItem(ItemDataSO itemData)
    {
        this.itemData = itemData;
        SetupVisuals();

        float xDropForce = Random.Range(-dropForce.x, dropForce.x);//生成一个随机的水平力，范围在-dropForce.x到dropForce.x之间
        rb.linearVelocity = new Vector2(xDropForce, dropForce.y);//将物品的速度设置为一个新的Vector2对象，水平分量为xForce，垂直分量为dropForce.y
        col.isTrigger = false;//将物品的Collider2D组件的isTrigger属性设置为false，使其成为一个物理碰撞体，可以与玩家发生碰撞
    }

    private void SetupVisuals()
    {
        sr.sprite = itemData.itemIcon;//设置物品的视觉效果，将物品数据的图标赋值给SpriteRenderer组件的sprite属性
        gameObject.name = "Object_ItemPickup - " + itemData.itemName;//设置物品的名称，将物品数据的名称赋值给游戏对象的名称
    }

    private void OnCollisionEnter2D(Collision2D collision)//当物品与其他物体发生碰撞时调用
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && col.isTrigger == false)
        {
            //如果碰撞的物体是地面，并且物品的Collider2D组件的isTrigger属性为false，将isTrigger属性设置为true，使物品成为一个触发器，可以与玩家发生触发事件
            col.isTrigger = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;//将物品的Rigidbody2D组件的constraints属性设置为FreezeAll，使物品停止移动和旋转
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Inventory_Player inventory = collision.GetComponent<Inventory_Player>();

        if (inventory == null)
            return;

        Inventory_Item itemToAdd = new Inventory_Item(itemData);//当玩家进入触发器时，创建一个新的Inventory_Item对象，并将物品数据赋值给它
        Inventory_Storage storage = inventory.storage;//当玩家进入触发器时，获取玩家的Inventory_Player组件，并从中获取玩家的Inventory_Storage组件

        //如果物品类型是Material，调用AddMaterialToStash方法将物品添加到玩家的材料仓库中，并销毁这个物品对象
        if (itemData.itemType == ItemType.Material)
        {
            storage.AddMaterialToStash(itemToAdd);
            Destroy(gameObject);
            return;
        }

        if (inventory.CanAddItem(itemToAdd))//如果玩家的背包可以添加这个物品，调用AddItem方法将物品添加到玩家的背包中，并销毁这个物品对象
        {
            inventory.AddItem(itemToAdd);
            Destroy(gameObject);
        }
    }
}
