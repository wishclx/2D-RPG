using UnityEngine;

public interface ICounterable
{
    public bool CanBeCountered { get; } // 定义一个属性，表示该对象是否可以被反击
    public void HandleCounter();// 定义一个方法，用于处理被反击的逻辑
}
