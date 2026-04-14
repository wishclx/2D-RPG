using UnityEngine;

/// <summary>
/// Entity_AnimationTriggers 的职责说明。
/// </summary>
public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
    }

    /// <summary>
    /// 执行 CurrentStateTrigger 逻辑。
    /// </summary>
    private void CurrentStateTrigger()
    {
        entity.CurrentStateAnimationTrigger();
    }

    /// <summary>
    /// 执行 AttackTrigger 逻辑。
    /// </summary>
    private void AttackTrigger()
    {
        entityCombat.PerformAttack();
    }
}
