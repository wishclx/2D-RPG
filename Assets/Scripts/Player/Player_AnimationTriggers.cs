using UnityEngine;

public class Player_AnimationTriggers : Entity_AnimationTriggers
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
    }

    private void ThrowSword() => player.skillManager.swordThrow.ThrowSword();// 这个方法被动画事件调用，用于执行剑抛出技能的实际投掷逻辑。
}
