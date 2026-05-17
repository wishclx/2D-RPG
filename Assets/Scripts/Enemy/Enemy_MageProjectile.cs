using UnityEngine;

public class Enemy_MageProjectile : MonoBehaviour
{
    private Entity_Combat combat;
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    [SerializeField] private float arcHeight = 2f;//抛物线的高度
    [SerializeField] private LayerMask whatCanCollideWith;//定义可以与弹道碰撞的层

    public void SetupProjectile(Transform target, Entity_Combat combat)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
        anim.enabled = false;//初始时禁用动画，直到碰撞发生时才启用
        this.combat = combat;

        Vector2 velocity = CalculateBallisticVelocity(transform.position, target.position);
        rb.linearVelocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatCanCollideWith) != 0)//检查碰撞的对象是否在可碰撞层中
        {
            //造成伤害的逻辑
            combat.PerformAttackOnTarget(collision.transform);//调用战斗组件的方法对目标造成伤害

            rb.linearVelocity = Vector2.zero;//停止弹道的运动
            rb.gravityScale = 0;//禁用重力，使弹道停留在碰撞点
            anim.enabled = true;
            col.enabled = false;//禁用碰撞器，防止再次碰撞
            Destroy(gameObject, 2f);
        }
    }

    //在Update中计算弹道轨迹并应用初速度
    private Vector2 CalculateBallisticVelocity(Vector2 start, Vector2 end)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);// 计算重力加速度绝对值
        if (gravity <= 0f)
            return Vector2.zero;// 重力为0时无法计算弹道，直接返回零速度

        float displacementX = end.x - start.x;// 水平位移
        float displacementY = end.y - start.y;// 竖直位移

        float safeArcHeight = Mathf.Max(0.01f, arcHeight);// 防止高度为0导致时间为0
        float apexHeight = Mathf.Max(start.y, end.y) + safeArcHeight;// 顶点高度高于起点/终点

        float heightToApex = apexHeight - start.y;// 起点到顶点的高度差
        float heightFromApex = apexHeight - end.y;// 顶点到终点的高度差

        float timeToApex = Mathf.Sqrt(2f * heightToApex / gravity);// 上升到顶点所需时间
        float timeFromApex = Mathf.Sqrt(2f * heightFromApex / gravity);// 从顶点下降到终点所需时间

        float totalTime = timeToApex + timeFromApex;// 总飞行时间
        if (totalTime <= 0f)
            return Vector2.zero;// 时间异常时直接返回零速度

        float velocityX = displacementX / totalTime;// 水平初速度
        float velocityY = gravity * timeToApex;// 竖直初速度

        return new Vector2(velocityX, velocityY);// 返回弹道初速度向量
    }
}
