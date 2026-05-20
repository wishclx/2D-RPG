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

    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Enemy_Health>();
        stats = GetComponent<Entity_Stats>();
        combat = GetComponent<Entity_Combat>();
        vfx = GetComponent<Entity_VFX>();
    }

    public void MakeUntargetable(bool canBeTargeted)
    {
        if (canBeTargeted == false)
            gameObject.layer = LayerMask.NameToLayer("Untargetable");// 将敌人设置为不可被玩家攻击的层，这样玩家的攻击就无法检测到敌人，从而使敌人暂时无法被攻击
        else
            gameObject.layer = LayerMask.NameToLayer("Enemy");// 将敌人设置回可被玩家攻击的层，使敌人可以再次被玩家攻击
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
    }
}





