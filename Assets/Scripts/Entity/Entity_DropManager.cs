using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity_DropManager : MonoBehaviour
{
    [SerializeField] private GameObject itemDropPrefab;//掉落物预制体
    [SerializeField] private ItemListDataSO dropData;//掉落数据

    [Header("Drop settings")]//掉落设置
    [SerializeField] private int maxRarityAmount = 1200;//最大稀有度数量
    [SerializeField] private int maxItemsToDrop = 3;//最大掉落物数量

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            DropItems();
    }

    public virtual void DropItems()//掉落物品
    {
        if (dropData == null)
        {
            Debug.Log(gameObject.name + "未分配掉落数据");
            return;
        }

        List<ItemDataSO> itemsToDrop = RollDrops();//随机掉落物列表
        int amountToDrop = Mathf.Min(itemsToDrop.Count, maxItemsToDrop);//实际掉落物数量

        for (int i = 0; i < amountToDrop; i++)
        {
            CreateItemDrop(itemsToDrop[i]);//创建掉落物
        }
    }

    protected void CreateItemDrop(ItemDataSO itemToDrop)
    {
        GameObject newItem = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);//在实体位置生成掉落物
        newItem.GetComponent<Object_ItemPickup>().SetupItem(itemToDrop);//设置掉落物数据
    }

    public List<ItemDataSO> RollDrops()//随机掉落物
    {
        List<ItemDataSO> possibleDrops = new List<ItemDataSO>();//可能掉落物列表
        List<ItemDataSO> finalDrops = new List<ItemDataSO>();//最终掉落物列表
        float maxRarityAmount = this.maxRarityAmount;//剩余的稀有度数量

        //首先根据掉落数据中的物品列表计算每个物品的掉落概率，并将可能掉落的物品添加到可能掉落物列表中
        foreach (var item in dropData.itemList)//遍历掉落数据中的物品列表
        {
            float dropChance = item.GetDropChance();//计算掉落概率

            if (Random.Range(0, 100) <= dropChance)//如果随机数小于等于掉落概率
                possibleDrops.Add(item);//将物品添加到可能掉落物列表
        }

        //然后根据稀有度降序排序可能掉落物列表
        possibleDrops = possibleDrops.OrderByDescending(item => item.itemRarity).ToList();//根据稀有度降序排序可能掉落物列表

        //最后从可能掉落物列表中选择掉落物，直到达到最大掉落物数量或没有更多的可能掉落物
        foreach (var item in possibleDrops)
        {
            if (maxRarityAmount > item.itemRarity)
            {
                finalDrops.Add(item);//将物品添加到最终掉落物列表
                maxRarityAmount -= item.itemRarity;//减少剩余的稀有度数量
            }
        }

        return finalDrops;//返回最终掉落物列表
    }

}
