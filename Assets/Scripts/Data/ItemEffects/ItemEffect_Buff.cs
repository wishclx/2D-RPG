using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Buff effect", fileName = "Item effect Data - buff")]
public class ItemEffect_Buff : ItemEffect_DataSO
{
    [SerializeField] private BuffEffectData[] buffsToApply;
    [SerializeField] private float duration;
    [SerializeField] private string source = Guid.NewGuid().ToString();//唯一标识符，确保每个buff效果都有一个独特的来源

    public override bool CanBeUsed(Player player)
    {

        if (player.stats.CanApplyBuffOf(source))
        {
            this.player = player;
            return true;
        }
        else
        {
            Debug.Log("部分BUFF不能重复运用!");
            return false;
        }
    }

    public override void ExecuteEffect()
    {
        player.stats.ApplyBuff(buffsToApply, duration, source);//应用buff效果
        player = null;//执行完效果后取消对玩家的引用，避免潜在的内存泄漏
    }
}
