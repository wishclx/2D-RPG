using UnityEngine;

public class Enemy_Slime : Enemy, ICounterable
{
    public bool CanBeCountered { get => canBeStunned; }
    public Enemy_SlimeDeadState slimeDeadState { get; set; }

    [Header("史莱姆")]
    [SerializeField] private GameObject slimeToCreatePrefab;//史莱姆分裂后生成的小史莱姆预制体
    [SerializeField] private int amountOfSlimesToCreate = 2;//史莱姆分裂后生成的小史莱姆数量
    [SerializeField] private Vector2 newSlimeVelocity;//史莱姆分裂后生成的小史莱姆的速度

    [SerializeField] private bool hasStunRecoveryAnimation = true;//史莱姆是否有眩晕恢复动画

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
        slimeDeadState = new Enemy_SlimeDeadState(this, stateMachine, "idle");

        anim.SetBool("hasStunRecovery", hasStunRecoveryAnimation);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override void EntityDeath()
    {
        stateMachine.ChangeState(slimeDeadState);
    }

    public void HandleCounter()
    {
        if (CanBeCountered == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }

    public void CreatSlimeOnDeath()
    {
        if (slimeToCreatePrefab == null)
            return;

        for (int i = 0; i < amountOfSlimesToCreate; i++)//根据设定的数量生成小史莱姆
        {
            GameObject newSlime = Instantiate(slimeToCreatePrefab, transform.position, Quaternion.identity);//在史莱姆当前位置生成一个新的史莱姆
            Enemy_Slime slimeScript = newSlime.GetComponent<Enemy_Slime>();

            slimeScript.SetupSlime(newSlimeVelocity, stats);//设置新史莱姆的速度和属性
        }
    }

    public void SetupSlime(Vector2 velocity, Entity_Stats newStats)
    {
        float xVelocity = velocity.x * Random.Range(-2, 2);//随机生成一个x轴速度，使得分裂后的小史莱姆可以向左右两个方向移动
        float yVelocity = velocity.y * Random.Range(1f, 2f);//随机生成一个y轴速度，使得分裂后的小史莱姆可以有不同的跳跃高度

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);//设置史莱姆的速度，使其分裂后的小史莱姆向相反方向移动

        stats.AdjustStatSetup(stats.resources, stats.offense, stats.defense, 0.6f, 1.2f);//调整史莱姆的属性
    }
}
