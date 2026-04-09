using UnityEngine;

public class Enemy_VFX : Entity_VFX
{
    [Header("Counter Attack Window")]
    [SerializeField] private GameObject attackAlert;// 攻击警告的游戏对象


    public void EnableAttackAlert(bool enable)
    {
        if (attackAlert == null)
            return;

        attackAlert.SetActive(enable);// 启用或禁用攻击警告的游戏对象
    }
}
