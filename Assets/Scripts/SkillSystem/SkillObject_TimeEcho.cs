using UnityEngine;

public class SkillObject_TimeEcho : SkillObject_Base
{
    [SerializeField] private float wispMoveSpeed = 15f;// 精灵分身的移动速度。
    [SerializeField] private GameObject onDeathVfx;
    [SerializeField] private LayerMask whatIsGround;// 地面层级，用于检测分身是否接触地面。
    private bool shouldMoveToPlayer = false;

    private Transform playerTransform;
    private Skill_TimeEcho echoManager;// 引用 Skill_TimeEcho 管理器，用于获取分身的持续时间等信息。
    private TrailRenderer wispTrail;
    private Entity_Health playerhealth;
    private SkillObject_Health echoHealth;
    private Player_SkillManager skillManager;
    private Entity_StatusHandler statusHandler;

    public int maxAttacks { get; private set; }

    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;
        playerStats = echoManager.player.stats;// 从 Skill_TimeEcho 管理器获取玩家的统计信息，并存储在 playerStats 属性中。
        damageScaleData = echoManager.damageScaleData;
        maxAttacks = echoManager.GetMaxAttacks();// 从 Skill_TimeEcho 管理器获取分身的最大攻击次数，并存储在 maxAttacks 属性中。
        playerTransform = echoManager.transform.root;// 获取 Skill_TimeEcho 管理器所在的根对象的 Transform 组件，并存储在 playerTransform 变量中。
        playerhealth = echoManager.player.health;
        skillManager = echoManager.skillManager;
        statusHandler = echoManager.player.statusHandler;

        Invoke(nameof(HandleDeath), echoManager.GetEchoDuration());
        FilpToTarget();// 调用 FilpToTarget 方法，使技能对象面向最近的目标。 

        echoHealth = GetComponent<SkillObject_Health>();
        wispTrail = GetComponentInChildren<TrailRenderer>();
        wispTrail.gameObject.SetActive(false);// 获取技能对象子对象中的 TrailRenderer 组件，并将其游戏对象设置为不激活状态，初始时不显示拖尾效果。

        anim.SetBool("canAttack", maxAttacks > 0);// 根据 maxAttacks 的值设置 Animator 中的 "canAttack" 参数，如果 maxAttacks 大于 0，则允许攻击。
    }

    private void Update()
    {
        if (shouldMoveToPlayer)
            HandleWispMovement();
        else
        {
            anim.SetFloat("yVelocity", rb.linearVelocity.y);// 将技能对象的 Animator 中的 "yVelocity" 参数设置为当前物体的垂直速度，以便根据速度调整动画状态。
            StopHorizontalMovement();// 调用 StopHorizontalMovement 方法，检查技能对象是否接触地面，并根据情况停止水平移动。
        }
    }

    private void HandlePlayerTouch()
    {
        // 计算治疗量，基于分身最后受到的伤害和 Skill_TimeEcho 管理器中定义的治疗百分比。
        float healAmount = echoHealth.lastDamageTaken * echoManager.GetPercentOfDamageHealed();
        playerhealth.IncreaseHealth(healAmount);// 将计算得到的治疗量应用到玩家的生命值上，调用玩家的 Entity_Health 组件的 IncreaseHealth 方法。

        float amountInSeconds = echoManager.GetCooldownReduceInSeconds();// 从 Skill_TimeEcho 管理器获取冷却时间减少量。
        skillManager.ReduceAllSkillCooldownBy(amountInSeconds);

        // 如果 Skill_TimeEcho 管理器指示分身可以移除负面效果，则调用玩家的 Entity_StatusHandler 组件的 RemoveNegativeEffects 方法，移除玩家身上的所有负面状态效果。
        if (echoManager.CanRemoveNegativeEffects())
            statusHandler.RemoveAllNegativeEffects();// 调用玩家的 Entity_StatusHandler 组件的 RemoveNegativeEffects 方法，移除玩家身上的所有负面状态效果。
    }

    private void HandleWispMovement()
    {
        // 将技能对象的位置逐渐移动到玩家的位置，使用 MoveTowards 方法以 wispMoveSpeed 的速度进行移动。
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, wispMoveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, playerTransform.position) < .5f)
        {
            HandlePlayerTouch();
            Destroy(gameObject);
        }
    }

    private void FilpToTarget()
    {
        Transform target = FindClosestTarget();// 调用 FindClosestTarget 方法找到最近的目标，并将其存储在 target 变量中。

        if (target != null && target.position.x < transform.position.x)
            transform.Rotate(0, 180, 0);// 如果目标在技能对象的左侧，则将技能对象旋转 180 度，使其面向目标。
    }

    public void PerformAttack()
    {
        DamageEnemiesInRadius(targetCheck, 1);// 调用 DamageEnemiesInRadius 方法，在 targetCheck 位置以半径 1 的范围内对敌人造成伤害。

        if (targetGotHit == false)// 如果没有敌人被击中，则直接返回，不执行后续的攻击逻辑。
            return;

        // 从 Skill_TimeEcho 管理器获取分身复制的概率，并生成一个随机值进行比较，确定是否可以复制分身。
        bool canDuplicate = Random.value < echoManager.GetDuplicateChance();
        float xOffset = transform.position.x < lastTarget.position.x ? 1f : -1f;// 根据技能对象与上一个目标的位置关系，确定复制分身时的水平偏移量。

        if (canDuplicate)
            // 如果可以复制分身，则调用 Skill_TimeEcho 管理器的 CreateTimeEcho 方法，在上一个目标的位置加上水平偏移量的位置创建一个新的分身。
            echoManager.CreateTimeEcho(lastTarget.position + new Vector3(xOffset, 0, 0));
    }

    public void HandleDeath()
    {
        Instantiate(onDeathVfx, transform.position, Quaternion.identity);// 在技能对象死亡时，在其位置生成一个预设的死亡特效。

        if (echoManager.ShouldBeWisp())
            TurnIntoWisp();
        else
            Destroy(gameObject);
    }

    private void TurnIntoWisp()
    {
        shouldMoveToPlayer = true;
        anim.gameObject.SetActive(false);
        wispTrail.gameObject.SetActive(true);// 如果 Skill_TimeEcho 管理器指示分身应该是一个精灵，则将拖尾效果的游戏对象设置为激活状态，显示拖尾效果。
        rb.simulated = false;// 将技能对象的 Rigidbody2D 组件的模拟属性设置为 false，使其不受物理引擎的影响，允许它自由移动到玩家位置。
    }

    private void StopHorizontalMovement()
    {
        // 从技能对象的位置向下发射一条射线，长度为 1.5f，检测是否与地面层级发生碰撞。
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, whatIsGround);

        if (hit.collider != null)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);// 如果射线检测到地面，则将技能对象的水平速度设置为 0，保持垂直速度不变。
    }
}
