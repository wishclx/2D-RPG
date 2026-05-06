using UnityEngine;

public class UI_CraftListButton : MonoBehaviour
{
    [SerializeField] private ItemListDataSO craftData;//制作品列表数据
    private UI_CraftSlot[] craftSlots;//列表槽位

    public void SetCraftSlots(UI_CraftSlot[] craftSlots) => this.craftSlots = craftSlots;

    public void UpdateCraftSlots()
    {
        if (craftSlots == null || craftSlots.Length == 0)
        {
            Debug.LogWarning("未设置可用的craftSlots");
            return;
        }

        if (craftData == null || craftData.itemList == null)
        {
            Debug.LogWarning("craftData 或 itemList 未设置");
            return;
        }

        // 清理所有槽位
        foreach (var slot in craftSlots)
        {
            slot.gameObject.SetActive(false);//先隐藏所有槽位
        }

        int showCount = Mathf.Min(craftData.itemList.Length, craftSlots.Length);//防止越界

        for (int i = 0; i < showCount; i++)
        {
            ItemDataSO itemData = craftData.itemList[i];//获取制作品数据
            craftSlots[i].gameObject.SetActive(true);//显示对应数量的槽位
            craftSlots[i].SetupButton(itemData);//设置每个槽位的内容，例如显示制作品信息
        }

        if (craftData.itemList.Length > craftSlots.Length)
            Debug.LogWarning($"制作品数量（{craftData.itemList.Length}）超过了可用UI槽位数量（{craftSlots.Length}），超出部分未显示");
    }
}
