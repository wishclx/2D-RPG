using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
/// <summary>
/// 单个属性值容器。
/// 支持基础值 + 修饰器（Buff / Debuff）的最终值计算。
/// </summary>
public class Stat
{
    [SerializeField] private float baseValue;// 属性基础值。
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();// 当前生效的修饰器列表。

    private bool needToCalculated = true;// 标记是否需要重新计算最终值。
    private float finalValue;// 缓存后的最终值。

    /// <summary>
    /// 执行 GetValue 逻辑。
    /// </summary>
    public float GetValue()
    {
        if (needToCalculated)
        {
            finalValue = GetFinalValue();// 数据有变更时重新计算。
            needToCalculated = false;// 计算完成后取消脏标记。
        }

        return finalValue;// 返回缓存的最终值。
    }

    /// <summary>
    /// 执行 AddModifier 逻辑。
    /// </summary>
    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd = new StatModifier(value, source);// 创建新的修饰器。
        modifiers.Add(modToAdd);// 加入修饰器列表。
        needToCalculated = true;// 标记需要重新计算。
    }

    /// <summary>
    /// 执行 RemoveModifier 逻辑。
    /// </summary>
    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source);// 按来源移除全部修饰器。
        needToCalculated = true;// 标记需要重新计算。
    }

    /// <summary>
    /// 执行 GetFinalValue 逻辑。
    /// </summary>
    private float GetFinalValue()
    {
        finalValue = baseValue;// 先从基础值开始。

        foreach (var modifier in modifiers)
        {
            finalValue += modifier.value;// 累加所有修饰器的数值。
        }

        return finalValue;
    }

    public void SetBaseValue(float value) => baseValue = value;
}

[Serializable]
/// <summary>
/// 属性修饰器。
/// `source` 用于标识来源，便于按来源移除。
/// </summary>
public class StatModifier
{
    public float value;
    public string source;// 修饰器来源标识。

    /// <summary>
    /// 执行 StatModifier 逻辑。
    /// </summary>
    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}
