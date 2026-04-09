using UnityEngine;

public class Player_DeadState : PlayerState
{
    public Player_DeadState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        input.Disable();// 禁止玩家输入
        rb.simulated = false;// 禁止物理模拟，使玩家角色不受物理影响
    }
}
