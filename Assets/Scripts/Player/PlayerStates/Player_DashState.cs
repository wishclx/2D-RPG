using UnityEngine;
using UnityEngine.XR;

public class Player_DashState : PlayerState
{
    private float originalGravityScale;
    private int dashDir;
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillsManager.dash.OnstartEffect();
        player.vfx.DoImageEchoEffect(player.dashDuration);

        dashDir = player.moveInput.x != 0 ? (int)player.moveInput.x : player.facingDir;
        stateTimer = player.dashDuration; // 设置 Dash 状态持续时间

        originalGravityScale = rb.gravityScale;// 记录原始重力缩放，以便在退出 Dash 状态时恢复
        rb.gravityScale = 0;

        player.health.SetCanTakeDamage(false);
        player.gameObject.layer = LayerMask.NameToLayer("Untargetable");// 设置玩家为不可被敌人攻击的层
    }

    public override void Update()
    {
        base.Update();
        CancelDashIfNeeded();
        player.SetVelocity(player.dashSpeed * dashDir, 0f);

        if (stateTimer < 0f)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        skillsManager.dash.OnEndEffect();

        player.health.SetCanTakeDamage(true);
        player.SetVelocity(0, 0);
        rb.gravityScale = originalGravityScale;

        player.gameObject.layer = LayerMask.NameToLayer("Player");// 恢复玩家的层为可被敌人攻击的层
    }


    public void CancelDashIfNeeded()
    {
        if (player.wallDetected)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.wallSlideState);
        }
    }
}


