using System.Collections.Generic;
using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{
    [SerializeField] private GameObject domainPrefab;

    [Header("Slowing Down Upgrade")]
    [SerializeField] private float slowDownPercent = .8f;//减速百分比
    [SerializeField] private float slowDownDomainDuration = 5f;//减速领域持续时间

    [Header("Shard Cast Upgrade")]
    [SerializeField] private int shardsToCast = 10;//施法次数 
    [SerializeField] private float shardCastDomainSlow = 1f;//施法领域减速百分比
    [SerializeField] private float shardCastDomainDuration = 8f;//施法领域持续时间
    private float spellCastTimer;//施法领域计时器
    private float spellsPerSecond;//每秒施法次数

    [Header("Time echo Cast Upgrade")]
    [SerializeField] private int echoToCast = 8;//施法次数
    [SerializeField] private float echoCastDomainSlow = 1f;//施法领域减速百分比
    [SerializeField] private float echoCastDomainDuration = 6f;//施法领域持续时间
    [SerializeField] private float healthToRestoreWithEcho = .05f;//每次回声施法时根据玩家最大生命值恢复的生命值百分比

    [Header("Domain Settings")]
    public float maxDomainDistance = 10f;
    public float expandSpeed = 3f;

    private List<Enemy> trappedTargets = new List<Enemy>();//被困在领域中的敌人列表
    private Transform currentTarget;//当前施法目标

    public void CreatDomain()
    {
        spellsPerSecond = GetSpellsToCast() / GetDomainDuration();//根据施法次数和领域持续时间计算每秒施法次数

        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<SkillObject_DomainExpansion>().SetupDomain(this);
    }

    public void DoSpellCasting()
    {
        spellCastTimer -= Time.deltaTime;//减少施法领域计时器

        if (currentTarget == null)
            currentTarget = FindTargetInDomain();//如果当前施法目标不存在了，就在领域中寻找一个新的目标

        if (currentTarget != null && spellCastTimer < 0)
        {
            CastSpell(currentTarget);//在当前施法目标身上施放一个法术
            spellCastTimer = 1f / spellsPerSecond;//重置施法领域计时器，根据每秒施法次数计算出下次施法的时间间隔
            currentTarget = null;//施法后重置当前施法目标，让下次施法时重新寻找目标
        }
    }

    private void CastSpell(Transform target)
    {
        if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
        {
            Vector3 offset = Random.value < .5f ? new Vector2(1, 0) : new Vector2(-1, 0);//在目标的左右两侧随机选择一个位置作为回声出现的位置
            skillManager.timeEcho.CreateTimeEcho(target.position + offset);
        }

        if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
        {
            skillManager.shard.CreateRawShard(target, true);//在目标位置创造一个碎片，true表示这个碎片是由领域展开技能创造的
        }
    }

    private Transform FindTargetInDomain()
    {
        trappedTargets.RemoveAll(target => target == null || target.health.isDead);//移除被困在领域中的敌人列表中已经死亡或不存在的敌人

        if (trappedTargets.Count == 0)
            return null;

        int randomIndex = Random.Range(0, trappedTargets.Count);//在被困在领域中的敌人列表中随机选择一个敌人作为施法目标
        return trappedTargets[randomIndex].transform;
    }

    public float GetDomainDuration()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
            return slowDownDomainDuration;
        else if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
            return shardCastDomainDuration;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
            return echoCastDomainDuration;

        return 0;
    }

    public float GetSlowPercentage()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
            return slowDownPercent;
        else if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
            return shardCastDomainSlow;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
            return echoCastDomainSlow;

        return 0;
    }

    private int GetSpellsToCast()
    {
        if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
            return shardsToCast;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
            return echoToCast;

        return 0;
    }

    public bool InstantDomain()//如果领域展开技能被激活，立即创造一个领域
    {
        return upgradeType != SkillUpgradeType.Domain_EchoSpam
            && upgradeType != SkillUpgradeType.Domain_ShardSpam;//如果不是回声领域和碎片领域，那么
    }

    public void AddTarget(Enemy targetToAdd)
    {
        trappedTargets.Add(targetToAdd);
    }

    public void ClearTargets()
    {
        foreach (var enemy in trappedTargets)
            enemy.StopSlowDown();//停止对被困在领域中的敌人施加的减速效果

        trappedTargets = new List<Enemy>();//清空被困在领域中的敌人列表
    }
}
