using System.Collections;
using UnityEngine;

public class Enemy_Mage : Enemy, ICounterable
{
    public bool CanBeCountered { get => canBeStunned; }
    public Enemy_MageRetreatState mageRetreatState { get; private set; }
    public Enemy_MageBattleState mageBattleState { get; private set; }
    public Enemy_MageSpellCastState mageSpellCastState { get; private set; }

    [Header("法师")]
    [SerializeField] private GameObject spellPrefab;//法术预制体
    [SerializeField] private Transform spellStartPosition;//法术生成位置
    [SerializeField] private int amountToCast = 3;//施法次数
    [SerializeField] private float spellCastCooldown = .3f;//施法间隔
    public bool spellCastPerformed { get; private set; }
    [Space]
    public float retreatCooldown = 5;//后退技能冷却时间
    public float retreatMaxDistance = 8;
    public float retreatSpeed = 15;
    [SerializeField] private Transform behindCollsionCheck;
    [SerializeField] private bool hasStunRecoveryAnimation = true;//法师是否有眩晕恢复动画

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        mageSpellCastState = new Enemy_MageSpellCastState(this, stateMachine, "spellCast");
        mageRetreatState = new Enemy_MageRetreatState(this, stateMachine, "battle");
        mageBattleState = new Enemy_MageBattleState(this, stateMachine, "battle");
        battleState = mageBattleState;

        anim.SetBool("hasStunRecovery", hasStunRecoveryAnimation);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public void SetSpellCastPerformed(bool performed) => spellCastPerformed = performed;//供动画事件调用，设置施法完成状态

    public override void SpecialAttack()
    {
        StartCoroutine(CastSpellCo());
    }

    private IEnumerator CastSpellCo()
    {
        for (int i = 0; i < amountToCast; i++)
        {
            Enemy_MageProjectile projectile
                = Instantiate(spellPrefab, spellStartPosition.position, Quaternion.identity).GetComponent<Enemy_MageProjectile>();

            projectile.SetupProjectile(player.transform, combat);
            yield return new WaitForSeconds(spellCastCooldown);//等待施法间隔
        }

        SetSpellCastPerformed(true);
    }

    public void HandleCounter()
    {
        if (CanBeCountered == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }

    public bool CanMoveBackwards()
    {
        //检查法师后方是否有墙或没有地面，如果有墙或没有地面则不能后退
        bool detectedWall = Physics2D.Raycast(behindCollsionCheck.position, Vector2.right * -facingDir, 1.5f, whatIsGround);
        bool noGround = Physics2D.Raycast(behindCollsionCheck.position, Vector2.down, 1.5f, whatIsGround) == false;

        return noGround || detectedWall;//如果没有地面或者检测到墙则返回true，表示不能后退
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(behindCollsionCheck.position,
            new Vector3(behindCollsionCheck.position.x + (1.5f * -facingDir), behindCollsionCheck.position.y));

        Gizmos.DrawLine(behindCollsionCheck.position,
            new Vector3(behindCollsionCheck.position.x, behindCollsionCheck.position.y - 1.5f));
    }
}
