using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : Entity_Stats
{

    private List<string> activeBuff = new List<string>();
    private Inventory_Player inventory;

    protected override void Awake()
    {
        base.Awake();
        inventory = GetComponent<Inventory_Player>();
    }

    public bool CanApplyBuffOf(string source)
    {
        return activeBuff.Contains(source) == false;//如果activeBuff中不包含该来源的buff，则可以应用
    }

    public void ApplyBuff(BuffEffectData[] buffsToApply, float duration, string source)
    {
        //启动一个协程，在持续时间内应用buff效果，并在结束时移除buff效果
        StartCoroutine(BuffCo(buffsToApply, duration, source));
    }

    private IEnumerator BuffCo(BuffEffectData[] buffsToApply, float duration, string source)
    {
        activeBuff.Add(source);//将buff来源添加到activeBuff列表中，表示该buff正在作用中

        foreach (var buff in buffsToApply)
        {
            GetStatByType(buff.type).AddModifier(buff.value, source);
        }

        yield return new WaitForSeconds(duration);

        foreach (var buff in buffsToApply)
        {
            GetStatByType(buff.type).RemoveModifier(source);//在持续时间结束后，移除buff效果
        }

        inventory.TriggerUpdateUI();//从玩家的UI中移除该buff的图标
        activeBuff.Remove(source);//将buff来源从activeBuff列表中移除，表示该buff不再作用中
    }
}
