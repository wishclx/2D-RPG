using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreviwSlot : MonoBehaviour
{
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialNameValue;

    //获取物品数据，当前拥有数量，所需数量
    public void SetupMaterialSlot(ItemDataSO itemData, int avaliableAmount, int requiredAmount)
    {
        materialIcon.sprite = itemData.itemIcon;
        materialNameValue.text = itemData.itemName + " - " + avaliableAmount + "/" + requiredAmount;
    }
}
