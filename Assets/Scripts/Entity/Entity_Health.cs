using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private Slider healthBar;// 定义一个Slider变量来存储血条组件的引用
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;

    [SerializeField] protected float currentHealth;
    [SerializeField] protected bool isDead;
    [Header("Health regen")]
    [SerializeField] private float regenInterval = 1f;// 自动回血的时间间隔
    [SerializeField] private bool canRegenerateHealth = true;// 是否可以自动回血

    [Header("On Damage Knockback")]
    [SerializeField] private float knockbackDuration = 0.2f;// 击退持续时间
    [SerializeField] private Vector2 onDamageKnockback = new Vector2(1.5f, 2.5f);// 受到伤害时的击退力度
    [Header("On Heavy Damage Knockback")]
    [Range(0, 1)]
    [SerializeField] private float heavyDamageThreshold = .3f;// 重击的伤害阈值，超过这个值的伤害将触发重击效果
    [SerializeField] private float heavyKnockbackDuration = .5f;// 重击的持续时间
    [SerializeField] private Vector2 onHeavyDamageKnockback = new Vector2(7f, 7f);// 重击时的击退力度

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        healthBar = GetComponentInChildren<Slider>();

        currentHealth = entityStats.GetMaxHealth();// 在Awake方法中将当前生命值设置为根据统计数据计算得到的最大生命值
        UpdateHealthBar();

        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
        // 使用InvokeRepeating方法来定期调用RegenerateHealth方法，实现自动回血功能，第一个参数是要调用的方法名，第二个参数是第一次调用的延迟时间，第三个参数是之后每次调用的间隔时间
    }

    public virtual bool TakeDamge(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (isDead)
            return false;

        if (AttackEvaded())
        {
            Debug.Log($"{gameObject.name} evaded the attack!");// 输出一个调试日志，显示哪个对象闪避了攻击
            return false;
        }

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;// 获取攻击者的护甲穿透率，如果攻击者没有Entity_Stats组件，则默认为0

        float mitigation = entityStats.GetArmorMitigation(armorReduction);// 获取护甲减伤率
        float physicalDamageTaken = damage * (1 - mitigation);// 计算最终伤害值，考虑护甲减伤

        float resistance = entityStats.GetElementalResistance(element);// 获取元素抗性
        float elementalDamageTaken = elementalDamage * (1 - resistance);// 计算最终元素伤害值，考虑元素抗性

        TakeKnockback(damageDealer, physicalDamageTaken);
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);
        //Debug.Log($"{gameObject.name} took {physicalDamageTaken} physical damage and {elementalDamageTaken} {element} elemental damage!");// 输出一个调试日志，显示哪个对象受到了多少物理伤害和元素伤害

        return true;
    }

    private bool AttackEvaded() => Random.Range(0, 100) < entityStats.GetEvasion();// 生成一个0到100之间的随机数，如果这个数小于实体的闪避率，则表示攻击被闪避了

    private void RegenerateHealth()
    {
        if (canRegenerateHealth == false)
            return;

        float regenAmount = entityStats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }

    private void IncreaseHealth(float healAmount)
    {
        if (isDead)
            return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();

        currentHealth = Mathf.Min(newHealth, maxHealth);// 将当前生命值增加治疗量，但不超过最大生命值
        UpdateHealthBar();
    }

    public void ReduceHealth(float damage)
    {
        entityVfx?.PlayOnDamageVfx();// 使用null条件运算符来调用PlayOnDamageVfx方法，如果entityVfx不为null，则调用该方法，否则跳过调用
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        entity.EntityDeath();// 调用Entity类中的EntityDeath方法来处理死亡逻辑
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null)
            return;
        healthBar.value = currentHealth / entityStats.GetMaxHealth(); // 更新血条的值，显示当前生命值占最大生命值的比例
    }

    private void TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);// 计算击退向量
        float duration = CalculateDuration(finalDamage);// 计算击退持续时间

        entity?.ReciveKnockback(knockback, duration);// 同样使用null条件运算符来调用ReciveKnockback方法，如果entity不为null，则调用该方法，否则跳过调用
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {

        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        // 根据伤害来源的位置来确定击退的方向，如果当前对象在伤害来源的右侧，则direction为1，表示向右击退；如果当前对象在伤害来源的左侧，则direction为-1，表示向左击退
        Vector2 knockback = IsHeavyDamage(damage) ? onHeavyDamageKnockback : onDamageKnockback;// 根据是否为重击来选择使用的击退力度

        knockback.x *= direction;// 将击退的x分量乘以方向，以确保击退的方向正确

        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    // 根据是否为重击来选择使用的击退持续时间

    private bool IsHeavyDamage(float damage) => damage / entityStats.GetMaxHealth() >= heavyDamageThreshold;
    // 判断是否为重击，根据伤害占当前生命值的百分比是否超过重击阈值来确定

}
