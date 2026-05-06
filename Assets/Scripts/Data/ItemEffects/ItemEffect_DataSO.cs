using UnityEngine;

public class ItemEffect_DataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription;
    protected Player player;

    public virtual bool CanBeUsed(Player player)
    {
        return true;
    }

    public virtual void ExecuteEffect()
    {

    }

    public virtual void Subscribe(Player player)//对玩家进行订阅，方便在执行效果时调用玩家的属性和方法
    {
        this.player = player;
    }

    public virtual void Unsubscribe()
    {

    }
}
