using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Reaper : Enemy, ICounterable
{
    public bool CanBeCountered { get => canBeStunned; }
    public Enemy_ReaperAttackState reaperAttackState { get; private set; }
    public Enemy_ReaperBattleState reaperBattleState { get; private set; }
    public Enemy_ReaperTeleportState reaperTeleportState { get; private set; }
    public Enemy_ReaperSpellCastState reaperSpellCastState { get; private set; }

    [Header("boss状态")]
    public float maxBattleIdleTime = 5;//在战斗状态中，敌人可能会进入一个短暂的闲置状态，这个参数定义了这个闲置状态的最大持续时间，增加不可预测性

    [Header("boss法术参数")]
    [SerializeField] private DamageScaleData damageScaleData;
    [SerializeField] private GameObject spellCastPrefab;//法术预制体，用于实例化法术对象
    [SerializeField] private int amountToCast = 6;//每次施法时生成的法术数量，增加攻击的威胁性和覆盖范围
    [SerializeField] private float spellCastRate = 1.2f;//施法的频率，控制敌人施法的节奏
    [SerializeField] private float spellCastStateCooldown = 10;
    [SerializeField] private Vector2 playerOffsetPrediction;//预测玩家位置的偏移量，用于让敌人的法术攻击更具挑战性，迫使玩家不断移动以避免被击中
    private float lastTimeCastedSpells = float.NegativeInfinity;//记录上次施法的时间，用于计算施法冷却时间
    public bool spellCastPreformed { get; private set; }//标志，指示敌人是否已经执行了施法动作，确保每次进入施法状态时只执行一次施法逻辑
    private Player playerScript;

    [Header("传送参数")]
    [SerializeField] private BoxCollider2D arenaBounds;//敌人所在的区域边界，用于限制传送点的范围
    [SerializeField] private float offsetCeterY = 1.7f;//传送点相对于敌人中心的垂直偏移量，确保传送点在敌人脚下而不是中心位置
    [SerializeField] private float chanceToTeleport = 0.25f;//每次攻击后传送的概率
    private float defaultTeleportChance;//默认的传送概率，用于重置传送概率
    public bool teleportTrigger { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
        moveState = new Enemy_MoveState(this, stateMachine, "move");

        reaperAttackState = new Enemy_ReaperAttackState(this, stateMachine, "attack");
        reaperBattleState = new Enemy_ReaperBattleState(this, stateMachine, "battle");
        reaperTeleportState = new Enemy_ReaperTeleportState(this, stateMachine, "teleport");
        reaperSpellCastState = new Enemy_ReaperSpellCastState(this, stateMachine, "spellCast");

        battleState = reaperBattleState;
    }

    protected override void Start()
    {
        base.Start();

        arenaBounds.transform.parent = null;//将区域边界从敌人对象中分离出来，确保它不会随敌人移动而移动
        defaultTeleportChance = chanceToTeleport;

        stateMachine.Initialize(idleState);
    }

    public void HandleCounter()
    {
        if (CanBeCountered == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }

    public override void SpecialAttack()
    {
        StartCoroutine(CastSpellCo());
    }

    private IEnumerator CastSpellCo()
    {
        if (playerScript == null)
            playerScript = player.GetComponent<Player>();

        for (int i = 0; i < amountToCast; i++)
        {

            bool playerMoving = playerScript.rb.linearVelocity.magnitude > 0;

            float xOffset = playerMoving ? playerOffsetPrediction.x * playerScript.facingDir : 0;
            Vector3 spellPosistion = player.transform.position + new Vector3(xOffset, playerOffsetPrediction.y);

            Enemy_ReaperSpell spell
                = Instantiate(spellCastPrefab, spellPosistion, Quaternion.identity).GetComponent<Enemy_ReaperSpell>();

            spell.SetupSpell(combat, damageScaleData);

            yield return new WaitForSeconds(spellCastRate);//等待施法间隔
        }

        SetSpellCastPreformed(true);
    }

    public void SetSpellCastPreformed(bool spellCastStatus) => spellCastPreformed = spellCastStatus;
    public bool CanDoSpellCast() => Time.time > lastTimeCastedSpells + spellCastStateCooldown;//检查敌人是否可以执行施法，基于上次施法时间和施法状态冷却时间
    public void SetSpellCastCooldown() => lastTimeCastedSpells = Time.time;//记录当前时间作为上次施法时间，用于施法冷却计算

    public bool ShouldTeleport()
    {
        if (Random.value < chanceToTeleport)
        {
            chanceToTeleport = defaultTeleportChance;//重置传送概率
            return true;
        }

        chanceToTeleport += .05f;//每次攻击后增加传送概率，增加玩家预期敌人会传送的感觉
        return false;
    }

    public void SetTeleportTrigger(bool triggerStatus) => teleportTrigger = triggerStatus;

    public Vector3 FindTeleportPoint()
    {
        int maxAttempts = 10;//最大尝试次数
        float bossWithColliderHalf = col.bounds.size.x / 2;//敌人宽度的一半，用于确保传送点不会生成在敌人碰撞体内

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(arenaBounds.bounds.min.x + bossWithColliderHalf,
                                                                        arenaBounds.bounds.max.x - bossWithColliderHalf);

            Vector2 raycastPoint = new Vector2(randomX, arenaBounds.bounds.max.y);//从区域上方开始发射射线，确保传送点在地面上

            RaycastHit2D hit = Physics2D.Raycast(raycastPoint, Vector2.down, Mathf.Infinity, whatIsGround);

            if (hit.collider != null)
                return hit.point + new Vector2(0, offsetCeterY);//如果射线击中了地面，返回击中点加上垂直偏移量作为传送点
        }

        return transform.position;//如果在最大尝试次数内没有找到合适的传送点，返回敌人当前的位置作为默认传送点
    }
}
