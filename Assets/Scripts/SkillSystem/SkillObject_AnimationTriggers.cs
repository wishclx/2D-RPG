using UnityEngine;

public class SkillObject_AnimationTriggers : MonoBehaviour
{
    private SkillObject_TimeEcho timeEcho;

    private void Awake()
    {
        timeEcho = GetComponentInParent<SkillObject_TimeEcho>();
    }

    private void AttackTrigger()
    {
        timeEcho.PerformAttack();// 当动画事件触发时，调用 SkillObject_TimeEcho 的 PerformAttack 方法，对敌人造成伤害。
    }

    private void TryTerminate(int currentAttackIndex)
    {
        if (currentAttackIndex == timeEcho.maxAttacks)
            // 当动画事件触发时，检查当前攻击索引是否达到最大攻击次数，如果是，则调用 SkillObject_TimeEcho 的 HandleDeath 方法，处理分身的死亡逻辑。
            timeEcho.HandleDeath();
    }
}
