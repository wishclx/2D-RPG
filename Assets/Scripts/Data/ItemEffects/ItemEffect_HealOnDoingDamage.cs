using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Heal on doing damage", fileName = "Item effect Data - Heal on doing phys damage")]
public class ItemEffect_HealOnDoingDamage : ItemEffect_DataSO
{
    [SerializeField] private float percentHealedOnAttack = .2f;//每次攻击造成伤害时，治疗玩家的百分比

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.combat.OnDoingPhysicalDamage += HealOnDoingDamage;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.combat.OnDoingPhysicalDamage -= HealOnDoingDamage;
        player = null;
    }

    private void HealOnDoingDamage(float damage)
    {
        player.health.IncreaseHealth(damage * percentHealedOnAttack);
    }
}
