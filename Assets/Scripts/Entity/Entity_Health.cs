using System;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamageable
{
    public event Action OnTakingDamage;
    public event Action OnHealthUpdate;


    private Slider healthBar;
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    private Entity_DropManager dropManager;

    private bool miniHealthBarActive;//开关血条
    [SerializeField] protected float currentHealth;
    [Header("Health regen")]
    [SerializeField] private float regenInterval = 1f;
    [SerializeField] private bool canRegenerateHealth = true;
    public float lastDamageTaken { get; private set; } // 上次受到伤害的时间，用于控制生命值再生的时机。
    public bool isDead { get; private set; }
    protected bool canTakeDamage = true;//是否可以受到伤害

    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7f, 7f);
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float heavyKnockbackDuration = .5f;
    [Header("On Heavy Damage Knockback")]
    [SerializeField] private float heavyDamageThreshold = .3f;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        healthBar = GetComponentInChildren<Slider>();
        dropManager = GetComponent<Entity_DropManager>();

    }

    protected virtual void Start()
    {
        SetupHealth();
    }

    private void SetupHealth()
    {

        if (entityStats == null)
            return;

        currentHealth = entityStats.GetMaxHealth();
        OnHealthUpdate += UpdateHealthBar;

        UpdateHealthBar();
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    public virtual bool TakeDamge(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (isDead || canTakeDamage == false)
            return false;

        if (AttackEvaded())
        {
            Debug.Log($"{gameObject.name} evaded the attack!");
            return false;
        }

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;

        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0;// 获取护甲减伤率
        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;

        float physicalDamageTaken = damage * (1 - mitigation);
        float elementalDamageTaken = elementalDamage * (1 - resistance);

        TakeKnockback(damageDealer, physicalDamageTaken);
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);

        lastDamageTaken = physicalDamageTaken + elementalDamageTaken;// 更新上次受到伤害的时间，以便控制生命值再生的时机。


        OnTakingDamage?.Invoke();// 触发受到伤害的事件，通知其他系统（如UI、音效等）进行相应的反应。
        return true;
    }

    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    private bool AttackEvaded()
    {
        if (entityStats == null)
            return false;
        else
            return UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();
    }

    private void RegenerateHealth()
    {
        if (canRegenerateHealth == false)
            return;

        float regenAmount = entityStats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }

    public void IncreaseHealth(float healAmount)
    {
        if (isDead)
            return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();

        currentHealth = Mathf.Min(newHealth, maxHealth);
        OnHealthUpdate?.Invoke();
    }

    public void ReduceHealth(float damage)
    {
        if (isDead) //已死亡则不再处理伤害，防止重复触发死亡逻辑
            return;

        currentHealth -= damage;

        entityVfx?.PlayOnDamageVfx();
        OnHealthUpdate?.Invoke();

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        entity.EntityDeath();
        dropManager?.DropItems();// 调用掉落管理器的掉落方法，生成掉落物。
    }

    public float GetHealthPercent() => currentHealth / entityStats.GetMaxHealth();// 返回当前生命值占最大生命值的百分比

    public void SetHealthToPercent(float percent)
    {
        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp(percent, 0, 1);// 将当前生命值设置为最大生命值的百分比，确保百分比在 0 到 1 之间。
        OnHealthUpdate?.Invoke();
    }

    public float GetCurrentHealth() => currentHealth;// 返回当前生命值的数值。

    private void UpdateHealthBar()
    {
        if (healthBar == null && healthBar.transform.parent.gameObject.activeSelf == false)// 如果血条组件不存在或者血条对象未激活，则不更新血条显示。
            return;

        healthBar.value = currentHealth / entityStats.GetMaxHealth();
    }

    public void EnableHealthBar(bool enable) => healthBar?.transform.parent.gameObject.SetActive(enable);//

    private void TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateDuration(finalDamage);

        entity?.ReciveKnockback(knockback, duration);
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {

        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;

        knockback.x *= direction;

        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    private bool IsHeavyDamage(float damage)
    {
        if (entityStats == null)
            return false;
        else
            return damage / entityStats.GetMaxHealth() >= heavyDamageThreshold;// 判断伤害是否超过最大生命值的某个百分比，以确定是否为重击。
    }
}
