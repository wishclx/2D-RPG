using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private ElementType currentEffect = ElementType.None;

    [Header("Shock effect details")]
    [SerializeField] private GameObject lightningStrikeVfx;// 感电触发雷击时播放的特效。
    [SerializeField] private float currentCharge;// 当前累积电荷。
    [SerializeField] private float maximumCharge = 1;// 电荷达到该值时触发雷击。
    private Coroutine shockCo;// 感电状态协程引用，用于重置计时。

    private void Awake()
    {
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
    }

    public void RemoveAllNegativeEffects()
    {
        StopAllCoroutines();// 停止所有状态效果协程，立即移除所有状态。
        currentEffect = ElementType.None;
        entityVfx.StopAllVfx();// 停止所有状态特效显示。
    }

    public void ApplyStatusEffect(ElementType element, ElementalEffectData effectData)
    {
        if (element == ElementType.Ice && CanBeApplied(ElementType.Ice))
            ApplyChillEffect(effectData.chillDuration, effectData.chillSlowMultiplier);

        if (element == ElementType.Fire && CanBeApplied(ElementType.Fire))
            ApplyBurnEffect(effectData.burnDuration, effectData.totalBurnDamage);

        if (element == ElementType.Lightning && CanBeApplied(ElementType.Lightning))
            ApplyShockEffect(effectData.shockDuration, effectData.shockDamage, effectData.shockCharge);
    }

    private void ApplyShockEffect(float duration, float damage, float charge)
    {
        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge * (1 - lightningResistance);// 根据雷抗计算实际增加电荷。
        currentCharge += finalCharge;

        if (currentCharge >= maximumCharge)
        {
            DoLightningStrike(damage);
            StopShockEffect();
            return;
        }

        if (shockCo != null)
            StopCoroutine(shockCo);// 若已有感电协程，先停止以重置持续时间。

        shockCo = StartCoroutine(ShockEffectCo(duration));// 启动新的感电持续计时。
    }

    private void StopShockEffect()
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        entityVfx.StopAllVfx();
    }

    private void DoLightningStrike(float damage)
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);// 生成雷击特效。
        entityHealth.ReduceHealth(damage);// 造成一次雷击伤害。
    }

    private IEnumerator ShockEffectCo(float duration)
    {
        currentEffect = ElementType.Lightning;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Lightning);// 播放感电状态特效。

        yield return new WaitForSeconds(duration);
        StopShockEffect();
    }

    private void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);
        float finalDamage = fireDamage * (1 - fireResistance);// 根据火抗计算最终总伤害。

        StartCoroutine(BurnEffectCo(duration, finalDamage));// 启动燃烧持续伤害。
    }

    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire);// 播放燃烧状态特效。

        int tickersPerSecond = 2;// 每秒触发伤害次数。
        int tickCount = Mathf.RoundToInt(tickersPerSecond * duration);// 计算总触发次数。

        float damagePerTick = totalDamage / tickCount;// 每次触发的伤害值。
        float tickInterval = 1f / tickersPerSecond;// 每次触发间隔。

        for (int i = 0; i < tickCount; i++)
        {
            if (entityHealth.isDead) //死亡后停止燃烧伤害
                break;

            entityHealth.ReduceHealth(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;
    }

    private void ApplyChillEffect(float duration, float slowMultiplier)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);// 获取冰抗。
        float finalDuration = duration * (1 - iceResistance);// 根据冰抗缩短减速持续时间。

        StartCoroutine(ChilledEffectCo(finalDuration, slowMultiplier));// 启动冰冻减速效果。
    }

    private IEnumerator ChilledEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);// 对实体施加减速。
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Ice);// 播放冰冻状态特效。

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
