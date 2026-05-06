using System.Collections.Generic;
using UnityEditor.Splines;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_Sword
{
    [SerializeField] private float bounceSpeed = 15;
    private int bounceCount;

    private Collider2D[] enemyTargets;
    private Transform nextTarget;// 反弹目标
    private List<Transform> selectedBefore = new List<Transform>();// 已经选中过的目标

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        anim.SetTrigger("spin");// 反弹剑的动画是旋转的，所以直接触发旋转动画就好了，不需要区分方向了
        base.SetupSword(swordManager, direction);

        bounceSpeed = swordManager.bounceSpeed;
        bounceCount = swordManager.bounceCount;
    }

    protected override void Update()
    {
        HandleComeback();// 先处理回归玩家的逻辑，如果正在回归玩家，就不处理反弹逻辑了
        HandleBounce();
    }

    private void HandleBounce()
    {
        if (nextTarget == null)
            return;

        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);// 向下一个目标移动

        if (Vector2.Distance(transform.position, nextTarget.position) < .75f)// 如果已经接近下一个目标了，就触发碰撞事件
        {
            DamageEnemiesInRadius(transform, 1);
            BounceToNextTarget();

            if (bounceCount == 0 || nextTarget == null)
            {
                nextTarget = null;// 如果反弹次数用完了，就不再有下一个目标了
                GetSwordBackToPlayer();// 直接回到玩家身边
            }

        }
    }

    private void BounceToNextTarget()// 反弹到下一个目标
    {
        nextTarget = GetNextTarget();
        bounceCount--;// 反弹次数减1
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyTargets == null)// 如果还没有获取到敌人目标列表，就获取一次
        {
            enemyTargets = GetEnemiesAround(transform, 10);// 获取周围一定范围内的敌人目标列表
            rb.simulated = false;// 先停止移动，等待选定下一个目标
        }

        DamageEnemiesInRadius(transform, 1);

        if (enemyTargets.Length <= 1 || bounceCount == 0)
            GetSwordBackToPlayer();// 如果周围没有足够的敌人了，就直接回到玩家身边
        else
            nextTarget = GetNextTarget();// 否则就选定下一个目标，准备反弹过去
    }

    private Transform GetNextTarget()// 获取下一个反弹目标
    {
        List<Transform> validTargets = GetValidTargets();// 获取当前可选的目标列表

        int randomIndex = Random.Range(0, validTargets.Count);// 从可选列表中随机选一个目标

        Transform nextTarget = validTargets[randomIndex];
        selectedBefore.Add(nextTarget);// 将选中的目标加入已选列表，避免重复选择

        return nextTarget;
    }

    private List<Transform> GetValidTargets()// 获取当前可选的目标列表，排除已经选中过的
    {
        List<Transform> validTargets = new List<Transform>();
        List<Transform> aliveTargets = GetAliveTargets();// 获取当前还存活的目标列表

        foreach (var enemy in aliveTargets)
        {
            if (enemy != null && !selectedBefore.Contains(enemy.transform))// 如果目标存在且没有被选中过，就加入可选列表
                validTargets.Add(enemy.transform);
        }

        if (validTargets.Count > 0)// 如果还有可选目标，就返回可选列表
            return validTargets;
        else
        {
            selectedBefore.Clear();
            return aliveTargets;// 如果没有可选目标了，就重置已选列表，重新从存活目标中选择
        }
    }

    private List<Transform> GetAliveTargets()// 获取当前还存活的目标列表
    {
        List<Transform> aliveTargets = new List<Transform>();

        foreach (var enemy in enemyTargets)
        {
            if (enemy != null)
                aliveTargets.Add(enemy.transform);
        }

        return aliveTargets;
    }

}
