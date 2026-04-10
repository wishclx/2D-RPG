using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public static event Action OnPlayerDeath;// 定义一个静态事件，当玩家死亡时触发

    private UI ui;// 玩家UI的引用，用于显示相关UI界面
    public PlayerInputSet input { get; private set; }
    public Player_SkillManager skillManager { get; private set; }

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
    private Coroutine queuedAttackCo;// 用于处理连击输入的协程

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

    protected override void Awake()
    {
        base.Awake();

        ui=FindAnyObjectByType<UI>();// 在场景中查找UI组件的引用
        input = new PlayerInputSet();
        skillManager = GetComponent<Player_SkillManager>();

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

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

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

        for (int i = 0; i < attackVelocity.Length; i++)// 遍历攻击速度数组，应用减速效果
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

    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke();// 触发玩家死亡事件，通知其他系统玩家已经死亡
        stateMachine.ChangeState(deadState);
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }
    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();//等待当前帧结束，确保所有输入事件都被处理完毕
        stateMachine.ChangeState(basicAttackState);
    }
    private void OnEnable()
    {
        input.Enable();//启用输入系统

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();//当玩家按下移动键时，读取输入值并存储在moveInput变量中
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;//当玩家松开移动键时，将moveInput变量重置为零

        input.Player.ToggleSkillTreeUI.performed += ctx => ui.ToggleSkillTreeUI();//当玩家按下切换技能树UI的键时，调用UI组件的方法切换技能树界面
    }

    private void OnDisable()
    {
        input.Disable();
    }

}
