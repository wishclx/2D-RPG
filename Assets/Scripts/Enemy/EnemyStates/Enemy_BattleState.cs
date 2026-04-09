using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private float lastTimeWasInBattle;

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        UpdateBattleTimer();//进入战斗状态时更新战斗计时器

        if (player == null)
            player = enemy.GetPlayerReference();// 如果玩家引用为null，通过敌人类的方法获取玩家的Transform引用
        //player ??= enemy.GetPlayerReference();

        if (ShouldRetreat())
        {
            rb.linearVelocity = new Vector2(enemy.retreatVelocity.x * -DirectionToPlayer(), enemy.retreatVelocity.y);//如果需要撤退，设置敌人的速度为撤退速度的相反方向
            enemy.HandleFlip(DirectionToPlayer());
        }
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerDetected())
            UpdateBattleTimer();

        if (BattleTimeOver())
            stateMachine.ChangeState(enemy.idleState);

        if (WithinAttackRange() && enemy.PlayerDetected())
            stateMachine.ChangeState(enemy.attackState);
        else
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocity.y);
    }

    private void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;//更新最后一次进入战斗状态的时间

    private bool BattleTimeOver() => Time.time - lastTimeWasInBattle > enemy.battleTimeDuration;//判断是否超过战斗时间，如果超过则返回true，否则返回false

    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;// 判断敌人和玩家之间的距离是否小于攻击距离

    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;//判断敌人和玩家之间的距离是否小于最小撤退距离

    private float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);//计算敌人和玩家之间的水平距离
    }

    private int DirectionToPlayer()
    {
        if (player == null)
            return 0;
        return player.position.x > enemy.transform.position.x ? 1 : -1;//判断玩家在敌人左边还是右边，返回1表示在右边，返回-1表示在左边
    }
}
