using System;
using UnityEngine;

[Serializable]
/// <summary>
/// Stat_DefenseGroup 的职责说明。
/// </summary>
public class Stat_DefenseGroup
{
    // 物理防御
    public Stat armor;//护甲
    public Stat evasion;

    // 元素抗性
    public Stat fireRes;
    public Stat iceRes;
    public Stat lightningRes;
}



