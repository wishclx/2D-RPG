using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    [Header("任务信息")]
    public string questTargetId;//任务目标ID，可以用来在保存和加载时识别任务目标，比如击败某个敌人，收集某个物品等

    [Header("金币掉落")]
    [SerializeField] private int goldDrop = 50;//敌人死亡时直接增加给玩家的金币数量
    public int GoldDrop => goldDrop;//提供只读访问

    public Entity_Stats stats { get; private set; }
    public Enemy_Health health { get; private set; }
    public Entity_Combat combat { get; private set; }
    public Entity_VFX vfx { get; private set; }
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;
    public Enemy_StunnedState stunnedState;

    [Header("战斗参数")]
    public float battleMoveSpeed = 3f;
    public float attackDistance = 2f;
    public float attackCooldown = .5f;//敌人每次攻击后需要等待的时间，单位为秒
    public bool canChasePlayer = true;//敌人是否会追逐玩家，如果为false，敌人只会在攻击范围内攻击玩家，不会追逐玩家
    [Space]
    public float battleTimeDuration = 5f;
    public float minRetreatDistance = 1f;
    public Vector2 retreatVelocity;

    [Header("反击参数")]
    public float stunnedDuration = 1f;
    public Vector2 stunnedVelocity = new Vector2(7f, 7f);
    [SerializeField] protected bool canBeStunned;

    [Header("移动参数")]
    public float idleTime = 2f;
    public float moveSpeed = 1.4f;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1f;

    [Header("玩家检测")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10;
    public Transform player { get; private set; }
    public float activeSlowMultiplier { get; private set; } = 1f;// 当前的减速倍率，默认为1（没有减速）

    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;
    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;

    // 引用计数：追踪有多少来源将敌人标记为不可被选中/无敌，避免重复覆盖或提早恢复
    private int untargetableRefCount = 0;

    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Enemy_Health>();
        stats = GetComponent<Entity_Stats>();
        combat = GetComponent<Entity_Combat>();
        vfx = GetComponent<Entity_VFX>();
    }

    /// <summary>
    /// 设置敌人是否可被选中/受伤。
    /// 使用引用计数避免重复调用导致恢复失败：每次传入 true 时增加计数，传入 false 时减少计数，
    /// 只有计数为0时才真正恢复为可被攻击层并允许受伤。
    /// 同时同步到生命组件的可受伤开关，保证伤害判定一致。
    /// </summary>
    public void MakeUntargetable(bool makeUntargetable)
    {
        if (makeUntargetable)
        {
            untargetableRefCount = Mathf.Max(0, untargetableRefCount) + 1;
        }
        else
        {
            untargetableRefCount = Mathf.Max(0, untargetableRefCount - 1);
        }

        bool isUntargetableNow = untargetableRefCount > 0;

        // 切换 Layer 保持原有行为（射线/过滤）
        gameObject.layer = isUntargetableNow ? LayerMask.NameToLayer("Untargetable") : LayerMask.NameToLayer("Enemy");

        // 同步到生命组件，确保伤害真正被禁止/允许（更可靠）
        if (health != null)
            health.SetCanTakeDamage(!isUntargetableNow);
    }

    /// <summary>
    /// 强制清理所有不可被选中标记（用于出错恢复/调试）
    /// </summary>
    public void ForceClearUntargetable()
    {
        untargetableRefCount = 0;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        if (health != null)
            health.SetCanTakeDamage(true);
    }

    public virtual void SpecialAttack()//敌人特有的攻击方式，可以在子类中重写实现不同的攻击行为
    {

    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {

        activeSlowMultiplier = 1 - slowMultiplier;

        anim.speed = anim.speed * activeSlowMultiplier;

        yield return new WaitForSeconds(duration);
        StopSlowDown();
    }

    public override void StopSlowDown()
    {
        activeSlowMultiplier = 1;
        anim.speed = 1;// 重置动画速度为正常速度
        base.StopSlowDown();
    }

    public void EnableCounterWindow(bool enable) => canBeStunned = enable;


    public override void EntityDeath()
    {
        base.EntityDeath();

        stateMachine.ChangeState(deadState);
    }


    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }

    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
            return;

        this.player = player;

        // 被攻击时立刻转身面对攻击者
        if (player != null)
        {
            int dirToPlayer = player.position.x > transform.position.x ? 1 : -1;
            if (dirToPlayer != facingDir)
                HandleFlip(dirToPlayer);
        }

        stateMachine.ChangeState(battleState);
    }

    public void DestoryGameObjectWithDelay(float delay = 10)
    {
        Destroy(gameObject, delay);//在敌人死亡后delay秒销毁敌人对象，给玩家足够的时间看到敌人的死亡动画和掉落物品
    }

    public Transform GetPlayerReference()
    {
        if (player == null)
            player = PlayerDetected().transform;

        return player;
    }

    public RaycastHit2D PlayerDetected()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;

        return hit;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * playerCheckDistance), playerCheck.position.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * attackDistance), playerCheck.position.y));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * minRetreatDistance), playerCheck.position.y));
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
        // 额外保险：确保禁用时清理不可被选中状态，防止残留
        ForceClearUntargetable();
    }

    public void TryEnterBattleState(Transform player, bool force)
    {
        // 如果需要可扩展的重载保留
        TryEnterBattleState(player);
    }

    public void DestroyGameObjectWithDelay(float delay = 10)
    {
        DestoryGameObjectWithDelay(delay);
    }

    public Transform GetPlayerReferenceSafe()
    {
        return GetPlayerReference();
    }
}





