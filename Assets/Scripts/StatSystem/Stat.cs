using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;//基础数值
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();//buff或debuff的数值

    private bool needToCalculated = true;//标记是否需要重新计算最终数值
    private float finalValue;//最终数值

    public float GetValue()
    {
        if (needToCalculated)
        {
            finalValue = GetFinalValue();//如果被修改过，重新计算最终数值
            needToCalculated = false;//标记为未修改
        }

        return finalValue;//返回最终数值
    }

    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd = new StatModifier(value, source);//创建一个新的modifier
        modifiers.Add(modToAdd);//添加一个新的modifier
        needToCalculated = true;//标记为已修改
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source);//移除所有来源为source的modifier
        needToCalculated = true;//标记为已修改
    }

    private float GetFinalValue()
    {
        finalValue = baseValue;//初始值为基础数值

        foreach (var modifier in modifiers)
        {
            finalValue += modifier.value;//将所有modifier的数值加到finalValue上
        }

        return finalValue;
    }

    public void SetBaseValue(float value) => baseValue = value;
}

[Serializable]
public class StatModifier
{
    public float value;
    public string source;//来源

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}
