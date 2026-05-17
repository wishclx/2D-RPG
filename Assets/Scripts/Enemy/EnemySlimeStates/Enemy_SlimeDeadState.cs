using UnityEngine;

public class Enemy_SlimeDeadState : Enemy_DeadState
{
    private Enemy_Slime enemySlime;

    public Enemy_SlimeDeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemySlime = enemy as Enemy_Slime;//将传入的Enemy对象转换为Enemy_Slime对象，以便在死状态中使用史莱姆特有的功能
    }

    public override void Enter()
    {
        base.Enter();

        //分裂史莱姆
        enemySlime.CreatSlimeOnDeath();
    }
}
