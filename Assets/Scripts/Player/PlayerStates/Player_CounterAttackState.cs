using UnityEngine;

/// <summary>
/// Player_CounterAttackState 的职责说明。
/// </summary>
public class Player_CounterAttackState : PlayerState
{
    private Player_Combat combat;
    private bool counteredSomebody;

    /// <summary>
    /// 执行 Player_CounterAttackState 逻辑。
    /// </summary>
    public Player_CounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }

    /// <summary>
    /// 执行 Enter 逻辑。
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        stateTimer = combat.GetCounterRecoveryDruation();
        counteredSomebody = combat.CounterAttackPerformed();

        anim.SetBool("counterAttackPerformed", counteredSomebody);
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, rb.linearVelocity.y);

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

        if (stateTimer <= 0 && counteredSomebody == false)
            stateMachine.ChangeState(player.idleState);
    }
}


