using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Heal effect", fileName = "Item effect Data - heal")]
public class ItemEffect_Heal : ItemEffect_DataSO
{
    [SerializeField] private float healPercent = .1f;//回复百分比

    public override void ExecuteEffect()
    {
        Player player = FindFirstObjectByType<Player>();

        float healAmount = player.stats.GetMaxHealth() * healPercent;

        player.health.IncreaseHealth(healAmount);//增加生命值
    }
}
