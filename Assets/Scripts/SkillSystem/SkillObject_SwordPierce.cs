using UnityEngine;

public class SkillObject_SwordPierce : SkillObject_Sword
{
    private int amountToPierce;// 穿刺数量

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);
        amountToPierce = swordManager.amountToPierce;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground");// 检测是否碰撞到地面

        if (amountToPierce <= 0 || groundHit)// 如果穿刺数量已经用完或者碰撞到地面，则停止剑的运动。
        {
            DamageEnemiesInRadius(transform, .3f);// 在剑停下的位置对周围的敌人造成伤害。
            StopSword(collision);
            return;
        }

        amountToPierce--;
        DamageEnemiesInRadius(transform, .3f);// 在剑穿刺的位置对周围的敌人造成伤害。
    }
}
