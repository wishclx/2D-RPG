using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private ElementType currentEffect = ElementType.None;

    [Header("Electrify effect details")]
    [SerializeField] private GameObject lightningStrikeVfx;// 定义一个GameObject变量来存储电击特效的预制体引用
    [SerializeField] private float currentCharge;// 定义一个float变量来存储当前的电荷值
    [SerializeField] private float maximumCharge = 1;// 定义一个float变量来存储最大电荷值，当当前电荷值达到或超过这个值时触发电击效果
    private Coroutine electrifyCo;// 定义一个Coroutine变量来存储电击效果的协程引用，以便在需要时停止协程


    private void Awake()
    {
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
    }

    public void ApplyElectrifyEffect(float duration, float damage, float charge)
    {
        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge * (1 - lightningResistance);// 根据电击抗性来计算实际的电荷增量，抗性越高电荷增量越低
        currentCharge += finalCharge;

        if (currentCharge >= maximumCharge)
        {
            DoLightningStrike(damage);
            StopElectrifyEffect();
            return;
        }

        if (electrifyCo != null)
            StopCoroutine(electrifyCo);// 如果当前正在播放电击特效的协程不为null，则停止该协程，以确保不会同时播放多个电击特效

        electrifyCo = StartCoroutine(ElectrifyEffectCo(duration));// 启动一个协程来处理电击状态的持续时间和特效播放，传入持续时间参数
    }

    private void StopElectrifyEffect()
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        entityVfx.StopAllVfx();
    }

    private void DoLightningStrike(float damage)
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);// 在实体位置实例化电击特效预制体
        entityHealth.ReduceHealth(damage);// 对实体造成伤害，调用Entity_Health组件的ReduceHp方法，传入伤害数值参数
    }

    private IEnumerator ElectrifyEffectCo(float duration)
    {
        currentEffect = ElementType.Lightning;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Lightning);// 调用Entity_VFX组件的方法来播放电击状态特效，传入持续时间和元素类型参数

        yield return new WaitForSeconds(duration);
        StopElectrifyEffect();
    }

    public void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);
        float finalDamage = fireDamage * (1 - fireResistance);// 根据火焰抗性来计算实际的总伤害，抗性越高总伤害越低

        StartCoroutine(BurnEffectCo(duration, finalDamage));// 启动一个协程来处理燃烧状态的持续时间和伤害计算
    }

    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire);// 调用Entity_VFX组件的方法来播放燃烧状态特效，传入持续时间和元素类型参数

        int tickersPerSecond = 2;// 定义每秒钟的伤害次数
        int tickCount = Mathf.RoundToInt(tickersPerSecond * duration);// 计算总的伤害次数，等于每秒钟的伤害次数乘以持续时间

        float damagePerTick = totalDamage / tickCount;// 计算每次伤害的数值，等于总伤害除以总的伤害次数
        float tickInterval = 1f / tickersPerSecond;// 计算每次伤害的时间间隔，等于1秒钟除以每秒钟的伤害次数

        for (int i = 0; i < tickCount; i++)
        {
            entityHealth.ReduceHealth(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;
    }

    public void ApplyChillEffect(float duration, float slowMultiplier)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);// 获取实体的冰冻抗性属性值
        float finalDuration = duration * (1 - iceResistance);// 根据冰冻抗性来计算实际的持续时间，抗性越高持续时间越短

        StartCoroutine(ChilledEffectCo(finalDuration, slowMultiplier));// 启动一个协程来处理冰冻状态的持续时间和特效播放
    }

    private IEnumerator ChilledEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);// 调用Entity组件的方法来减速实体，传入持续时间和减速倍率参数
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Ice);// 调用Entity_VFX组件的方法来播放冰冻状态特效，传入持续时间和元素类型参数

        yield return new WaitForSeconds(duration);

        currentEffect = ElementType.None;
    }

    public bool CanBeApplied(ElementType element)
    {
        if (element == ElementType.Lightning && currentEffect == ElementType.Lightning)
            return true;

        return currentEffect == ElementType.None;
    }
}
