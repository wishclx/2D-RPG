using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    protected Transform player;
    protected Transform lastTarget;
    protected float lastTimeWasInBattle;
    protected float lastTimeAttacked = float.NegativeInfinity;//记录上次攻击的时间，初始值为负无穷大，确保敌人一开始就可以攻击
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        UpdateBattleTimer();//进入战斗状态时更新战斗计时器

        if (player == null)
            player = enemy.GetPlayerReference();// 如果玩家引用为null，通过敌人类的方法获取玩家的Transform引用
        // player ??= enemy.GetPlayerReference();

        if (ShouldRetreat())
        {
            ShortRetreat();
        }
    }

    protected void ShortRetreat()
    {
        float x = (enemy.retreatVelocity.x * enemy.activeSlowMultiplier) * -DirectionToPlayer();
        float y = enemy.retreatVelocity.y;

        rb.linearVelocity = new Vector2(x, y);//如果需要后撤，设置敌人的速度为后撤速度的相反方向
        enemy.HandleFlip(DirectionToPlayer());
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerDetected())
        {
            UpdateTargetIfNeed();
            UpdateBattleTimer();
        }

        if (BattleTimeOver())
            stateMachine.ChangeState(enemy.idleState);

        if (WithinAttackRange() && enemy.PlayerDetected() && CanAttack())
        {
            lastTimeAttacked = Time.time;
            stateMachine.ChangeState(enemy.attackState);
        }
        else
        {
            float xVelocity = enemy.canChasePlayer ? enemy.GetBattleMoveSpeed() : 0.0001f;
            enemy.SetVelocity(xVelocity * DirectionToPlayer(), rb.linearVelocity.y);
        }
    }

    //判断敌人是否可以攻击，如果当前时间与上次攻击时间的差值大于攻击冷却时间，则返回true，否则返回false
    protected bool CanAttack() => Time.time - lastTimeAttacked > enemy.attackCooldown;

    protected void UpdateTargetIfNeed()
    {
        if (enemy.PlayerDetected() == false)
            return;

        Transform newTarget = enemy.PlayerDetected().transform;

        if (newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }
    }

    protected void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;//更新最后一次进入战斗状态的时间

    protected bool BattleTimeOver() => Time.time - lastTimeWasInBattle > enemy.battleTimeDuration;//判断是否超过战斗时间，如果超过则返回true，否则返回false

    protected bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;// 判断敌人和玩家之间的距离是否小于攻击距离

    protected bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;//判断敌人和玩家之间的距离是否小于最小撤退距离

    protected float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);//计算敌人和玩家之间的水平距离
    }

    protected int DirectionToPlayer()
    {
        if (player == null)
            return 0;
        return player.position.x > enemy.transform.position.x ? 1 : -1;//判断玩家在敌人左边还是右边，返回1表示在右边，返回-1表示在左边
    }
}
