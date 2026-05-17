using UnityEngine;

public class Enemy_ArcherArrow : MonoBehaviour, ICounterable
{
    [SerializeField] private LayerMask whatIsTarget;

    private Collider2D col;
    private Rigidbody2D rb;
    private Entity_Combat combat;
    private bool hasBeenCountered;
    private Animator anim;

    public bool CanBeCountered => true;

    public void SetupArrow(float xVelocity, Entity_Combat combat)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();

        rb.linearVelocity = new Vector2(xVelocity, 0);
        this.combat = combat;//设置箭矢的初始速度和关联的战斗组件

        if (rb.linearVelocity.x < 0)
            transform.Rotate(0, 180, 0);//根据箭矢的水平速度方向旋转箭矢，使其朝向正确的方向)
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //当箭矢碰撞到目标时，检查是否造成伤害
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)//检查碰撞的对象是否在目标层中
        {
            //造成伤害的逻辑
            combat.PerformAttackOnTarget(collision.transform);//调用战斗组件的方法对目标造成伤害
            StuckIntoTarget(collision.transform);//命中后停留在目标上
        }
    }

    private void StuckIntoTarget(Transform target)
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;//将箭矢的速度设为零，并将其设置为Kinematic，使其停留在目标上
        col.enabled = false;//禁用箭矢的碰撞器，使其不会再与其他对象发生碰撞
        anim.enabled = false;

        transform.parent = target;//将箭矢设置为目标的子对象，使其随目标移动

        Destroy(gameObject, 3f);
    }

    public void HandleCounter()
    {
        if (hasBeenCountered)
            return;

        hasBeenCountered = true;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x * -1, 0);//当箭矢被反击时，反转其水平速度，使其飞回敌人
        transform.Rotate(0, 180, 0);//旋转箭矢，使其朝向相反的方向

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");

        whatIsTarget = whatIsTarget | (1 << enemyLayer);//将敌人层添加到箭矢的目标层中，使其能够对敌人造成伤害
        whatIsTarget = whatIsTarget & ~(1 << playerLayer);//反击后移除玩家层，避免仍被命中

        transform.position += new Vector3(Mathf.Sign(rb.linearVelocity.x) * 0.1f, 0f, 0f);//把箭推出玩家碰撞体，避免重叠触发伤害
    }
}
