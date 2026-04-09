using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack details")]
    [SerializeField] private float counterRecovery = .1f;

    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;

        foreach (var target in GetDetectedColliders())
        {
            ICounterable counterable = target.GetComponent<ICounterable>(); // 获取目标上的ICounterable组件

            if (counterable == null)
                continue; // 如果目标没有ICounterable组件，跳过当前循环，继续检查下一个目标

            if (counterable.CanBeCountered)// 如果目标可以被反击
            {
                counterable.HandleCounter(); // 调用目标的HandleCounter方法，执行被反击的逻辑
                hasPerformedCounter = true; // 设置hasCounteredSombody为true，表示成功反击了某个目标
            }
        }

        return hasPerformedCounter;
    }

    public float GetCounterRecoveryDruation() => counterRecovery;
}
