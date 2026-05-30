using UnityEngine;

public class Enemy_MageRetreatState : EnemyState
{
    private Enemy_Mage enemyMage;
    private Vector3 startPosition;
    private Transform player;

    public Enemy_MageRetreatState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyMage = enemy as Enemy_Mage;//向下转型为Enemy_Mage以访问特定于法师的属性
    }

    public override void Enter()
    {
        base.Enter();

        if (player == null)
            player = enemy.GetPlayerReference();//如果玩家引用为null，通过敌人类的方法获取玩家的Transform引用

        startPosition = enemy.transform.position;//记录敌人开始后撤的位置

        rb.linearVelocity = new Vector2(enemyMage.retreatSpeed * -DirectionToPlayer(), 0);//设置敌人的速度为后撤速度的相反方向
        enemy.HandleFlip(DirectionToPlayer());//根据玩家的位置调整敌人的朝向

        //enemy.MakeUntargetable(true);//使敌人无法被攻击)
        enemy.vfx.DoImageEchoEffect(1);
    }

    public override void Update()
    {
        base.Update();

        //如果敌人已经后撤到最大距离，或者玩家不再被检测到，或者敌人无法继续后撤，则切换回战斗状态
        bool reachedMaxDistance = Vector2.Distance(enemy.transform.position, startPosition) >= enemyMage.retreatMaxDistance;

        if (reachedMaxDistance || enemyMage.CanMoveBackwards())//如果达到最大距离或者无法继续后撤
            stateMachine.ChangeState(enemyMage.mageSpellCastState);//切换到施法状态
    }

    public override void Exit()
    {
        base.Exit();
        enemy.vfx.StopImageEchoEffect();//停止后撤的视觉效果
        //enemy.MakeUntargetable(false);//使敌人可以被攻击
    }

    protected int DirectionToPlayer()
    {
        if (player == null)
            return 0;
        return player.position.x > enemy.transform.position.x ? 1 : -1;//判断玩家在敌人左边还是右边，返回1表示在右边，返回-1表示在左边
    }
}
