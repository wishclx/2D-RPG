using UnityEngine;

public class Enemy_ReaperSpell : MonoBehaviour
{
    private Entity_Combat combat;
    private DamageScaleData damageScaleData;

    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private Collider2D col;

    public void SetupSpell(Entity_Combat combat, DamageScaleData damageScaleData)
    {
        this.combat = combat;
        this.damageScaleData = damageScaleData;
        Destroy(gameObject, 2f);//2秒后销毁
    }

    private void EnableCollider() => col.enabled = true;
    private void DisableCollider() => col.enabled = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //确保只对指定的目标层进行攻击
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)
        {
            //执行攻击逻辑，传入碰撞对象的Transform以便攻击系统能够正确处理伤害和效果 
            combat.PerformAttackOnTarget(collision.transform, damageScaleData);
            DisableCollider();
        }
    }
}
