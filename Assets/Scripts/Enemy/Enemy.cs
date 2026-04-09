using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;
    public Enemy_StunnedState stunnedState;

    [Header("Battle details")]
    public float battleMoveSpeed = 3f;
    public float attackDistance = 2f;
    public float battleTimeDuration = 5f;
    public float minRetreatDistance = 1f;
    public Vector2 retreatVelocity;

    [Header("Stunned state deatails")]
    public float stunnedDuration = 1f;
    public Vector2 stunnedVelocity = new Vector2(7f, 7f);
    [SerializeField] protected bool canBeStunned;

    [Header("Movement details")]
    public float idleTime = 2f;
    public float moveSpeed = 1.4f;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1f;

    [Header("Player detection")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck; // 检测玩家的点
    [SerializeField] private float playerCheckDistance = 10; // 检测玩家的距离
    public Transform player { get; private set; }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalBattleSpeed = battleMoveSpeed;
        float originalAnimSpeed = anim.speed;

        float speedMultiplier = 1 - slowMultiplier;// 计算减速后的速度乘数，例如slowMultiplier为0.5时，speedMultiplier将为0.5，表示速度减半

        moveSpeed = moveSpeed * speedMultiplier;// 应用减速效果，调整移动速度
        battleMoveSpeed = battleMoveSpeed * speedMultiplier;
        anim.speed = anim.speed * speedMultiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;// 恢复原始速度
        battleMoveSpeed = originalBattleSpeed;
        anim.speed = originalAnimSpeed;
    }

    public void EnableCounterWindow(bool enable) => canBeStunned = enable;// 启用或禁用被击晕的窗口，允许敌人被击晕或不被击晕

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
        stateMachine.ChangeState(battleState);
    }

    public Transform GetPlayerReference()
    {
        if (player == null)
            player = PlayerDetected().transform;// 如果玩家引用为null，通过检测到玩家的位置来设置玩家的Transform，若没有检测到玩家，player将保持为null

        return player;
    }

    public RaycastHit2D PlayerDetected()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatIsGround);//检测玩家

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))//如果没有检测到玩家或者检测到的不是玩家，返回默认值
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
        Player.OnPlayerDeath += HandlePlayerDeath;// 订阅玩家死亡事件，当玩家死亡时调用HandlePlayerDeath方法
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;// 取消订阅玩家死亡事件，防止内存泄漏或意外调用
    }
}
