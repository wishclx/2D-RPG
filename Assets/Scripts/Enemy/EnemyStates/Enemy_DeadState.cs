using UnityEngine;

public class Enemy_DeadState : EnemyState
{
    private Collider2D col;

    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        col = enemy.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        anim.enabled = false; // 禁用动画组件，使敌人保持当前的死亡姿势
        col.enabled = false;// 禁用碰撞器，使敌人不会与玩家或其他对象发生碰撞

        rb.gravityScale = 12f; // 增加重力，使敌人快速下落 
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15);// 给敌人一个向上的初始速度，使其在死亡时有一个弹跳效果

        stateMachine.SwitchOffStateMachine();
    }
}
