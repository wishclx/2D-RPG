using UnityEngine;

/// <summary>
/// EntityState 的职责说明。
/// </summary>
public abstract class EntityState
{
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rb;
    protected Entity_Stats stats;

    protected float stateTimer;
    protected bool triggerCalled;

    /// <summary>
    /// 执行 EntityState 逻辑。
    /// </summary>
    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }
    /// <summary>
    /// 执行 Enter 逻辑。
    /// </summary>
    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime; 
        UpdateAnimationParameters();
    }

    /// <summary>
    /// 执行 Exit 逻辑。
    /// </summary>
    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }

    /// <summary>
    /// 执行 AnimationTrigger 逻辑。
    /// </summary>
    public void AnimationTrigger()
    {
        triggerCalled = true;
    }

    /// <summary>
    /// 执行 UpdateAnimationParameters 逻辑。
    /// </summary>
    public virtual void UpdateAnimationParameters()
    {
        
    }

    /// <summary>
    /// 执行 SyncAttackSpeed 逻辑。
    /// </summary>
    public void SyncAttackSpeed()
    {
        float attackSpeed = stats.offense.attackSpeed.GetValue();
        anim.SetFloat("attackSpeedMultilplier", attackSpeed);
    }
}


