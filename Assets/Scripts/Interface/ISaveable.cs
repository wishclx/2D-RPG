using UnityEngine;

public interface ISaveable
{
    public void LoadData(GameData data);//从GameData对象加载数据
    public void SaveData(ref GameData data);
}
