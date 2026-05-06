using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Material item", fileName = "Material Data - ")]
public class ItemDataSO : ScriptableObject
{
    public string saveID;//保存ID，唯一标识一个物品数据，可以用来在保存和加载时识别物品

    [Header("商店")]
    [Range(0, 10000)]
    public int itemPrice = 100;//物品价格
    public int minStackSizeAtShop = 1;//商店物品最小数量
    public int maxStackSizeAtShop = 1;//商店物品最大数量

    [Header("掉落")]
    [Range(0, 1000)]
    public int itemRarity = 100;//物品稀有度
    [Range(0, 100)]
    public float dropChance;//掉落几率
    [Range(0, 100f)]
    public float maxDropChance = 65;//最大掉落几率

    [Header("制造")]
    public Inventory_Item[] craftRecipe;//合成配方

    [Header("物品效果")]
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public int maxStackSize = 1;

    [Header("物品效果")]
    public ItemEffect_DataSO itemEffect;

    private void OnValidate()
    {
        dropChance = GetDropChance();

#if UNITY_EDITOR 
        string path = AssetDatabase.GetAssetPath(this);
        saveID = AssetDatabase.AssetPathToGUID(path);
#endif
    }

    public float GetDropChance()
    {
        float maxRarity = 1000f;//最大稀有度
        float chance = (maxRarity - itemRarity + 1) / maxRarity * 100f;//根据稀有度计算掉落几率，稀有度越高，掉落几率越低

        return Mathf.Min(maxDropChance, chance);
    }

}
