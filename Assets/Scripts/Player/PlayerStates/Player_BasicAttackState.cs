using UnityEngine;

public class Player_BasicAttackState : PlayerState
{
    private float attackVelocityTimer;
    private float lastTimeAttacked;// 上一次攻击的时间

    private bool comboAttackQueued;
    private int attackDir;
    private int comboIndex = 1;
    private int comboLimit = 3; // 连击的最大索引
    private const int FirstComboIndex = 1;// 第一个攻击动画的索引



    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)
        {
            Debug.LogWarning("Combo limit has been adjusted to match the number of attack velocities defined in the player settings.");
            comboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIndexIfNeeded();
        SyncAttackSpeed();

        attackDir = player.moveInput.x != 0 ? (int)player.moveInput.x : player.facingDir; // 如果有水平输入，使用输入方向，否则使用玩家面朝的方向

        //if (player.moveInput.x != 0)
        //    attackDir = ((int)player.moveInput.x);
        //else
        //    attackDir = player.facingDir;

        anim.SetInteger("basicAttackIndex", comboIndex);// 设置当前攻击动画索引
        ApplyAttackVelocity();
    }


    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (input.Player.Attack.WasPressedThisFrame())
            QueueNextAttack();

        if (triggerCalled)
            HandleStateExit();
    }

    public override void Exit()
    {
        base.Exit();
        comboIndex++;// 增加连击索引
        lastTimeAttacked = Time.time;// 记录这次攻击的时间
    }

    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false);// 先关闭当前攻击动画
            player.EnterAttackStateWithDelay(); // 通过玩家的方法进入下一个攻击状态，这样可以重置动画状态机的触发器
        }
        else
            stateMachine.ChangeState(player.idleState);
    }

    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
            comboAttackQueued = true;
    }

    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if (attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1]; // 获取当前攻击动画对应的速度

        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }

    private void ResetComboIndexIfNeeded()
    {
        if (Time.time - lastTimeAttacked > player.comboResetTime || comboIndex > comboLimit)
            comboIndex = FirstComboIndex;
    }
}
