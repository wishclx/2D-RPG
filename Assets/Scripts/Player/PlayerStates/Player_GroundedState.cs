using UnityEngine;

public class Player_GroundedState : PlayerState
{
    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0 && player.groundDetected == false)
            stateMachine.ChangeState(player.fallState);

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.jumpState);

        if (input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.basicAttackState);

        if (input.Player.CounterAttack.WasPressedThisFrame())
            stateMachine.ChangeState(player.counterAttackState);

        // 如果玩家按下远程攻击输入并且剑抛出技能可用，切换到剑抛出状态。
        if (input.Player.RangeAttack.WasPressedThisFrame() && skillsManager.swordThrow.CanUseSkill())
            stateMachine.ChangeState(player.swordThrowState);
    }
}
