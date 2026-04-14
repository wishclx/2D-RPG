using UnityEngine;

/// <summary>
/// Player_Combat 的职责说明。
/// </summary>
public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack details")]
    [SerializeField] private float counterRecovery = .1f;

    /// <summary>
    /// 执行 CounterAttackPerformed 逻辑。
    /// </summary>
    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;

        foreach (var target in GetDetectedColliders())
        {
            ICounterable counterable = target.GetComponent<ICounterable>(); 

            if (counterable == null)
                continue; 

            if (counterable.CanBeCountered)
            {
                counterable.HandleCounter(); 
                hasPerformedCounter = true; 
            }
        }

        return hasPerformedCounter;
    }

    public float GetCounterRecoveryDruation() => counterRecovery;
}


