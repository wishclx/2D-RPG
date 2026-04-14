using UnityEngine;

/// <summary>
/// Enemy_VFX 的职责说明。
/// </summary>
public class Enemy_VFX : Entity_VFX
{
    [Header("Counter Attack Window")]
    [SerializeField] private GameObject attackAlert;


    /// <summary>
    /// 执行 EnableAttackAlert 逻辑。
    /// </summary>
    public void EnableAttackAlert(bool enable)
    {
        if (attackAlert == null)
            return;

        attackAlert.SetActive(enable);
    }
}


