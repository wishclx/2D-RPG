using UnityEngine;

/// <summary>
/// StateMachine 的职责说明。
/// </summary>
public class StateMachine
{
    public EntityState currentState { get; private set; }
    public bool canChangeState;

    /// <summary>
    /// 执行 Initialize 逻辑。
    /// </summary>
    public void Initialize(EntityState startingState)
    {
        canChangeState = true;// 初始化时允许状态切换
        currentState = startingState;
        currentState.Enter();
    }

    /// <summary>
    /// 执行 ChangeState 逻辑。
    /// </summary>
    public void ChangeState(EntityState newState)
    {
        if (canChangeState == false)
            return;

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    /// <summary>
    /// 执行 UpdateActiveState 逻辑。
    /// </summary>
    public void UpdateActiveState()
    {
        currentState.Update();
    }

    public void SwitchOffStateMachine() => canChangeState = false;// 关闭状态机，禁止状态切换
}
