using UnityEngine;

/// <summary>
/// PlayerState 的职责说明。
/// </summary>
public abstract class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSet input;
    protected Player_SkillManager skillsManager;

    /// <summary>
    /// 执行 PlayerState 逻辑。
    /// </summary>
    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
        stats = player.stats;
        skillsManager = player.skillManager;
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (input.Player.Dash.WasPerformedThisFrame() && CanDash())
        {
            skillsManager.dash.SetSkillOnCooldown();
            stateMachine.ChangeState(player.dashState);
        }

        if (input.Player.UltimateSpell.WasPressedThisFrame() && skillsManager.domainExpansion.CanUseSkill())
        {
            if (skillsManager.domainExpansion.InstantDomain())//如果领域展开技能被激活，立即创造一个领域
            {
                skillsManager.domainExpansion.CreatDomain();
            }
            else
            {
                stateMachine.ChangeState(player.domainExpansionState);//否则进入领域展开状态，在那里创造领域
            }

            skillsManager.domainExpansion.SetSkillOnCooldown();//领域展开技能都进入冷却
        }
    }

    /// <summary>
    /// 执行 UpdateAnimationParameters 逻辑。
    /// </summary>
    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }


    /// <summary>
    /// 执行 CanDash 逻辑。
    /// </summary>
    private bool CanDash()
    {
        if (skillsManager.dash.CanUseSkill() == false)
            return false;

        if (player.wallDetected)
            return false;

        if (stateMachine.currentState == player.dashState || stateMachine.currentState == player.domainExpansionState)
            return false;

        return true;
    }
}
