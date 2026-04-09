using UnityEngine;

public class StateMachine
{
    public EntityState currentState { get; private set; }
    public bool canChangeState;

    public void Initialize(EntityState startingState)
    {
        canChangeState = true;// ³õÊ¼»¯Ê±ÔÊÐí×´Ì¬ÇÐ»»
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        if (canChangeState == false)
            return;

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateActiveState()
    {
        currentState.Update();
    }

    public void SwitchOffStateMachine() => canChangeState = false;// ¹Ø±Õ×´Ì¬»ú£¬½ûÖ¹×´Ì¬ÇÐ»»
}
