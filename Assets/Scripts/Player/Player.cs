using System;
using System.Collections;
using UnityEngine;

//玩家实体，包含玩家特有的属性和方法。
public class Player : Entity
{
    public static Player instance;// 玩家类的单例实例，方便其他脚本访问玩家对象。(只有一个玩家对象时使用单例模式)
    public static event Action OnPlayerDeath;// 玩家死亡时触发，供 UI/系统监听。

    public UI ui { get; private set; }// 场景中的 UI 引用。
    public PlayerInputSet input { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public Player_VFX vfx { get; private set; }
    public Entity_Health health { get; private set; }
    public Entity_StatusHandler statusHandler { get; private set; }
    public Player_Combat combat { get; private set; }
    public Inventory_Player inventory { get; private set; }
    public Player_Stats stats { get; private set; }
    public Player_QuestManager questManager { get; private set; }

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
    public Player_SwordThrowState swordThrowState { get; private set; }
    public Player_DomainExpansionState domainExpansionState { get; private set; }

    #endregion

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1f;
    private Coroutine queuedAttackCo;// 延迟进入攻击状态的协程引用。

    [Header("Ultimate ability details")]
    public float riseSpeed = 25;
    public float riseMaxDistance = 3;

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
    public Vector2 mousePosistion { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        instance = this;

        ui = FindAnyObjectByType<UI>();// 查找场景中的 UI 组件。
        vfx = GetComponent<Player_VFX>();
        health = GetComponent<Entity_Health>();
        skillManager = GetComponent<Player_SkillManager>();
        statusHandler = GetComponent<Entity_StatusHandler>();
        combat = GetComponent<Player_Combat>();
        inventory = GetComponent<Inventory_Player>();
        stats = GetComponent<Player_Stats>();
        questManager = GetComponent<Player_QuestManager>();

        input = new PlayerInputSet();
        ui.SetupControlsUI(input);// 将输入映射传递给 UI 以设置相关的 UI 控件。

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
        swordThrowState = new Player_SwordThrowState(this, stateMachine, "swordThrow");
        domainExpansionState = new Player_DomainExpansionState(this, stateMachine, "jumpFall");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public void TeleportPlayer(Vector3 posistion) => transform.position = posistion;// 直接将玩家传送到指定位置。

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


    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke();// 触发玩家死亡事件，供 UI/系统监听。
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
        yield return new WaitForEndOfFrame();// 等待本帧输入处理结束后再切攻击状态。
        stateMachine.ChangeState(basicAttackState);
    }

    private void TryInteract()// 尝试与玩家周围的可交互对象进行交互。
    {
        Transform closest = null;// 存储最近的可交互对象。
        float closestDistance = Mathf.Infinity;// 初始化为无穷大。
        Collider2D[] objectsAround = Physics2D.OverlapCircleAll(transform.position, 1f);// 获取玩家周围一定范围内的所有碰撞体。

        foreach (var target in objectsAround)
        {
            IInteractable interactable = target.GetComponent<IInteractable>();// 检查碰撞体是否具有可交互组件。
            if (interactable == null)
                continue;

            float distance = Vector2.Distance(transform.position, target.transform.position);// 计算与玩家的距离。

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = target.transform;// 更新最近的可交互对象。
            }
        }

        if (closest == null)
            return;

        closest.GetComponent<IInteractable>().Interact();// 调用最近可交互对象的交互方法。
    }

    private void OnEnable()
    {
        input.Enable();// 启用输入映射。

        input.Player.Mouse.performed += ctx => mousePosistion = ctx.ReadValue<Vector2>();// 读取鼠标位置输入。

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();// 读取移动输入。
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;// 松开按键时清空移动输入。

        input.Player.Spell.performed += ctx => skillManager.shard.TryUseSkill();// 触发当前法术(碎片)
        input.Player.Spell.performed += ctx => skillManager.timeEcho.TryUseSkill();//触发当前法术(分身)

        input.Player.Interact.performed += ctx => TryInteract();// 触发交互尝试。

        input.Player.QuickItemSlot1.performed += ctx => inventory.TryUseQuickItemInSlot(1);// 使用快键槽1中的物品。
        input.Player.QuickItemSlot2.performed += ctx => inventory.TryUseQuickItemInSlot(2);// 使用快键槽2中的物品。
    }

    private void OnDisable()
    {
        input.Disable();
    }

}
