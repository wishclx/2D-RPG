using UnityEngine;

public class SkillObject_DomainExpansion : SkillObject_Base
{
    private Skill_DomainExpansion domainManager;

    private float expandSpeed = 2f;//扩张速度
    private float duration;

    private float slowDownPercent = .9f;//减速百分比

    private Vector3 targetScale;
    private bool isShrinking;

    public void SetupDomain(Skill_DomainExpansion domainManager)
    {
        this.domainManager = domainManager;

        duration = domainManager.GetDomainDuration();
        slowDownPercent = domainManager.GetSlowPercentage();
        expandSpeed = domainManager.expandSpeed;
        float maxSize = domainManager.maxDomainDistance;

        targetScale = Vector3.one * maxSize;
        Invoke(nameof(ShrinkDomain), duration);
    }

    private void Update()
    {
        HandleScaling();
    }

    private void HandleScaling()
    {
        float sizeDiffrence = Mathf.Abs(transform.localScale.x - targetScale.x);//当前大小与目标大小的差距
        bool shouldChangeScale = sizeDiffrence > 0.1f;//如果差距大于0.1则继续调整大小

        if (shouldChangeScale)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * expandSpeed);//平滑调整大小

        if (isShrinking && sizeDiffrence <= 0.1f)
            TerminateDomain();
    }

    private void TerminateDomain()
    {
        domainManager.ClearTargets();//清除领域中的目标
        Destroy(gameObject);
    }

    private void ShrinkDomain()
    {
        isShrinking = true;
        targetScale = Vector3.zero;//缩小到无
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();//尝试获取敌人组件

        if (enemy == null)
            return;

        domainManager.AddTarget(enemy);
        enemy.SlowDownEntity(duration, slowDownPercent, true);//如果是敌人则施加减速效果
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();//尝试获取敌人组件

        if (enemy == null)
            return;

        enemy.StopSlowDown();//如果是敌人则移除减速效果
    }
}
