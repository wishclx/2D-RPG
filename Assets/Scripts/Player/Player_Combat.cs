using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("反击参数")]
    [SerializeField] private float counterRecovery = .1f;//反击后恢复的时间，单位为秒
    [SerializeField] private LayerMask whatIsCounterable;

    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;

        foreach (var target in GetDetectedColliders(whatIsCounterable))
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


