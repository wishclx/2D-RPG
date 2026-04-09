using System;
using UnityEngine;

[Serializable]
public class Stat_DefenseGroup
{
    //物理防御
    public Stat armor;//护甲
    public Stat evasion;//闪避

    //元素抵抗
    public Stat fireRes;//火焰抗性
    public Stat iceRes;
    public Stat lightningRes;
}
