using UnityEngine;

public class Enemy_Archer : Enemy
{
    public bool CanBeCountered { get => canBeStunned; }
    public Enemy_ArcherBattleState archerBattleState { get; private set; }

    [Header("弓箭参数")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowStartPoint;
    [SerializeField] private float arrowSpeed = 8;

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        archerBattleState = new Enemy_ArcherBattleState(this, stateMachine, "battle");
        battleState = archerBattleState;
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override void SpecialAttack()
    {
        GameObject newArrow = Instantiate(arrowPrefab, arrowStartPoint.position, Quaternion.identity);//在指定位置生成箭矢预制体
        newArrow.GetComponent<Enemy_ArcherArrow>().SetupArrow(facingDir * arrowSpeed, combat);//设置箭矢的初始速度和关联的战斗组件
    }

    public void HandleCounter()
    {
        if (CanBeCountered == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }

}
