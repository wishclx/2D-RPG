using UnityEngine;

/// <summary>
/// Player_BasicAttackState 的职责说明。
/// </summary>
public class Player_BasicAttackState : PlayerState
{
    private float attackVelocityTimer;
    private float lastTimeAttacked;// 记录上次攻击的时间，用于判断是否需要重置连击索引

    private bool comboAttackQueued;
    private int attackDir;
    private int comboIndex = 1;
    private int comboLimit = 3;
    private const int FirstComboIndex = 1;



    /// <summary>
    /// 执行 Player_BasicAttackState 逻辑。
    /// </summary>
    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)
        {
            Debug.LogWarning("Combo limit has been adjusted to match the number of attack velocities defined in the player settings.");
            comboLimit = player.attackVelocity.Length;
        }
    }

    /// <summary>
    /// 执行 Enter 逻辑。
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIndexIfNeeded();
        SyncAttackSpeed();

        attackDir = player.moveInput.x != 0 ? (int)player.moveInput.x : player.facingDir;

        // if (player.moveInput.x != 0)
        // attackDir = ((int)player.moveInput.x);
        // else
        // attackDir = player.facingDir;

        anim.SetInteger("basicAttackIndex", comboIndex);// 设置当前攻击索引，以便动画系统可以根据这个索引播放不同的攻击动画
        ApplyAttackVelocity();
    }


    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (input.Player.Attack.WasPressedThisFrame())
            QueueNextAttack();

        if (triggerCalled)
            HandleStateExit();
    }

    /// <summary>
    /// 执行 Exit 逻辑。
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastTimeAttacked = Time.time;
    }

    /// <summary>
    /// 执行 HandleStateExit 逻辑。
    /// </summary>
    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay();
        }
        else
            stateMachine.ChangeState(player.idleState);
    }

    /// <summary>
    /// 执行 QueueNextAttack 逻辑。
    /// </summary>
    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
            comboAttackQueued = true;
    }

    /// <summary>
    /// 执行 HandleAttackVelocity 逻辑。
    /// </summary>
    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if (attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    /// <summary>
    /// 执行 ApplyAttackVelocity 逻辑。
    /// </summary>
    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];

        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }

    /// <summary>
    /// 执行 ResetComboIndexIfNeeded 逻辑。
    /// </summary>
    private void ResetComboIndexIfNeeded()
    {
        if (Time.time - lastTimeAttacked > player.comboResetTime || comboIndex > comboLimit)
            comboIndex = FirstComboIndex;
    }
}


