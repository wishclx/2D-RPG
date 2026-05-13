using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Ice blaston", fileName = "Item effect Data - Ice blast on taking damage ")]
public class ItemEffect_IceBlastOnTakingDamage : ItemEffect_DataSO
{
    [SerializeField] private ElementalEffectData effectData;//冰爆状态效果数据
    [SerializeField] private float iceDamage;
    [SerializeField] private LayerMask whatIsEnemy;//敌人图层

    [Space]
    [SerializeField] private float healthPercentTrigger = .3f;//生命值触发百分比
    [SerializeField] private float cooldown;//冷却时间
    private float lastTimeUsed = -999;//上次使用时间，初始值为一个很小的负数，确保第一次使用时不会受到冷却时间的限制
    [Header("Vfx Objects")]
    [SerializeField] private GameObject iceBlastVfx;
    [SerializeField] private GameObject onHitVfx;


    public override void ExecuteEffect()
    {
        bool noCooldown = Time.time >= lastTimeUsed + cooldown;//检查是否冷却时间已到
        bool reachedThreshold = player.health.GetHealthPercent() <= healthPercentTrigger;//检查是否达到生命值触发百分比

        if (noCooldown && reachedThreshold)
        {
            //vfx
            player.vfx.CreateEffectOf(iceBlastVfx, player.transform);//在玩家位置创建冰爆特效
            lastTimeUsed = Time.time;
            DamageEnemiesWithIce();//对周围敌人造成冰元素伤害并应用状态效果
        }
    }

    private void DamageEnemiesWithIce()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, 1.5f, whatIsEnemy);//在玩家周围范围内检测敌人

        foreach (var target in enemies)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();//尝试获取敌人的IDamageable组件

            if (damageable == null)
                continue;//如果没有IDamageable组件，跳过这个敌人

            bool targetGotHit = damageable.TakeDamge(0, iceDamage, ElementType.Ice, player.transform);//对敌人造成冰元素伤害

            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();//尝试获取敌人的状态处理器组件
            statusHandler?.ApplyStatusEffect(ElementType.Ice, effectData);//如果状态处理器存在，应用冰元素状态效果

            if (targetGotHit)
                player.vfx.CreateEffectOf(onHitVfx, target.transform);//如果敌人被击中，在敌人位置创建命中特效
        }
    }

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);

        lastTimeUsed = -999f;// 每次订阅时重置冷却，避免上次运行残留

        if (this.player == null || this.player.health == null)
            return;

        this.player.health.OnTakingDamage -= ExecuteEffect;// 先移除，防止重复订阅
        this.player.health.OnTakingDamage += ExecuteEffect;// 受伤时触发冰爆逻辑
    }

    public override void Unsubscribe()
    {
        if (player != null && player.health != null)
            player.health.OnTakingDamage -= ExecuteEffect;// 取消订阅，防止悬挂事件

        base.Unsubscribe();
        player = null;// 清理引用
    }
}
