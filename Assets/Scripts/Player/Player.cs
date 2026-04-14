using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家实体。
/// 负责玩家状态机初始化、输入绑定与死亡/攻击等行为调度。
/// </summary>
public class Player : Entity
{
    public static event Action OnPlayerDeath;// 玩家死亡时触发，供 UI/系统监听。

    private UI ui;// 场景中的 UI 引用。
    public PlayerInputSet input { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public Player_VFX vfx { get; private set; }

    #region State Variables

    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }
    public Player_DeadState deadState { get; private set; }
    public Player_CounterAttackState counterAttackState { get; private set; }

    #endregion

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1f;
    private Coroutine queuedAttackCo;// 延迟进入攻击状态的协程引用。

    [Header("Movement details")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Vector2 wallJumpForce;
    [Range(0, 1)]
    public float inAirMoveMultiplier = .7f;
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = .7f;
    public float dashDuration = .25f;
    public float dashSpeed = 20f;
    public Vector2 moveInput { get; private set; }

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        ui = FindAnyObjectByType<UI>();// 查找场景中的 UI 组件。
        input = new PlayerInputSet();
        skillManager = GetComponent<Player_SkillManager>();
        vfx = GetComponent<Player_VFX>();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
        deadState = new Player_DeadState(this, stateMachine, "dead");
        counterAttackState = new Player_CounterAttackState(this, stateMachine, "counterAttack");
    }

    /// <summary>
    /// 执行 Start 逻辑。
    /// </summary>
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public void TeleportPlayer(Vector3 posistion) => transform.position = posistion;// 直接将玩家传送到指定位置。

    /// <summary>
    /// 执行 SlowDownEntityCo 逻辑。
    /// </summary>
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalWallJump = wallJumpForce;
        Vector2 originalJumpAttack = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = attackVelocity;

        float speedMultiplier = 1 - slowMultiplier;

        moveSpeed = moveSpeed * speedMultiplier;
        jumpForce = jumpForce * speedMultiplier;
        anim.speed = anim.speed * speedMultiplier;
        wallJumpForce = wallJumpForce * speedMultiplier;
        jumpAttackVelocity = jumpAttackVelocity * speedMultiplier;

        for (int i = 0; i < attackVelocity.Length; i++)// 对每段攻击位移速度应用减速系数。
        {
            attackVelocity[i] = attackVelocity[i] * speedMultiplier;
        }

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;
        anim.speed = originalAnimSpeed;
        wallJumpForce = originalWallJump;
        jumpAttackVelocity = originalJumpAttack;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = originalAttackVelocity[i];
        }
    }

    /// <summary>
    /// 执行 EntityDeath 逻辑。
    /// </summary>
    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke();// 广播玩家死亡事件。
        stateMachine.ChangeState(deadState);
    }

    /// <summary>
    /// 执行 EnterAttackStateWithDelay 逻辑。
    /// </summary>
    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }
    /// <summary>
    /// 执行 EnterAttackStateWithDelayCo 逻辑。
    /// </summary>
    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();// 等待本帧输入处理结束后再切攻击状态。
        stateMachine.ChangeState(basicAttackState);
    }
    /// <summary>
    /// 执行 OnEnable 逻辑。
    /// </summary>
    private void OnEnable()
    {
        input.Enable();// 启用输入映射。

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();// 读取移动输入。
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;// 松开按键时清空移动输入。

        input.Player.ToggleSkillTreeUI.performed += ctx => ui.ToggleSkillTreeUI();// 切换技能树界面。
        input.Player.Spell.performed += ctx => skillManager.shard.TryUseSkill();// 触发当前法术（锐化碎片）。
    }

    /// <summary>
    /// 执行 OnDisable 逻辑。
    /// </summary>
    private void OnDisable()
    {
        input.Disable();
    }

}
