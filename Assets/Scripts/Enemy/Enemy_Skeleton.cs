using UnityEngine;

/// <summary>
/// Enemy_Skeleton 的职责说明。
/// </summary>
public class Enemy_Skeleton : Enemy, ICounterable
{
    public bool CanBeCountered { get => canBeStunned; }
    

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
    }

    /// <summary>
    /// 执行 Start 逻辑。
    /// </summary>
    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    /// <summary>
    /// 执行 HandleCounter 逻辑。
    /// </summary>
    public void HandleCounter()
    {
        if (CanBeCountered == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }

}


