using UnityEngine;

public class Player_CounterAttackState : PlayerState
{
    private Player_Combat combat;
    private bool counteredSomebody;

    public Player_CounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();// 获取玩家的Player_Combat组件，以便在反击状态中调用相关方法
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = combat.GetCounterRecoveryDruation();
        counteredSomebody = combat.CounterAttackPerformed();// 调用Player_Combat组件中的CounterAttackPerformed方法，执行反击逻辑，并返回是否成功反击了某个目标

        anim.SetBool("counterAttackPerformed", counteredSomebody);// 设置动画参数，根据是否成功反击来切换动画状态
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, rb.linearVelocity.y);// 在反击状态中，玩家的水平速度被设置为0，但垂直速度保持不变，以确保玩家在空中时不会被重置垂直速度

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

        if (stateTimer <= 0 && counteredSomebody == false)
            stateMachine.ChangeState(player.idleState);
    }
}
