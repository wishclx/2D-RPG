using UnityEngine;

/// <summary>
/// Player_WallSlideState 的职责说明。
/// </summary>
public class Player_WallSlideState : PlayerState
{
    /// <summary>
    /// 执行 Player_WallSlideState 逻辑。
    /// </summary>
    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();
        HandleWallSlide();


        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.wallJumpState);

        if (player.wallDetected == false)
            stateMachine.ChangeState(player.fallState);

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);

            if(player.facingDir != player.moveInput.x)
                player.Flip();
        }
    }

    /// <summary>
    /// 执行 HandleWallSlide 逻辑。
    /// </summary>
    private void HandleWallSlide()
    {
        if (player.moveInput.y < 0)
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y);
        else
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y * player.wallSlideSlowMultiplier);
    }
}
