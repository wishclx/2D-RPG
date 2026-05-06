using UnityEngine;

public class Player_DomainExpansionState : PlayerState
{
    private Vector2 originalPosition;//初始位置
    private float orginalGravity;//初始重力
    private float maxDistanceToGoUp;//最终上升距离

    private bool isLevitating;//是否正在上升
    private bool createdDomain;//是否已经创建领域

    public Player_DomainExpansionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        originalPosition = player.transform.position;
        orginalGravity = rb.gravityScale;
        maxDistanceToGoUp = GetAvalibaleRiseDistance(); //计算最终上升距离

        player.SetVelocity(0, player.riseSpeed);
        player.health.SetCanTakeDamage(false);
    }

    public override void Update()
    {
        base.Update();

        //如果未达到最大上升距离且未开始上升，则继续上升
        if (Vector2.Distance(originalPosition, player.transform.position) >= maxDistanceToGoUp && isLevitating == false)
            Levitate();

        if (isLevitating)
        {
            skillsManager.domainExpansion.DoSpellCasting();//在领域中施放法术

            if (stateTimer <= 0)
            {
                isLevitating = false;
                rb.gravityScale = orginalGravity;
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        createdDomain = false;
        player.health.SetCanTakeDamage(true);
    }

    private void Levitate()
    {
        isLevitating = true;
        rb.linearVelocity = Vector2.zero;//停止上升
        rb.gravityScale = 0;//关闭重力

        stateTimer = skillsManager.domainExpansion.GetDomainDuration();//设置领域持续时间

        if (createdDomain == false)
        {
            createdDomain = true;
            //在这里创建领域，可以调用领域的生成方法，或者直接实例化领域预制体。
            skillsManager.domainExpansion.CreatDomain();
        }
    }

    private float GetAvalibaleRiseDistance()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(player.transform.position, Vector2.up, player.riseMaxDistance, player.whatIsGround);

        return hit.collider != null ? hit.distance - 1 : player.riseMaxDistance;//如果上方有障碍物，返回距离减去1(人物中心与障碍物之间的距离)，否则返回最大上升距离
    }
}
