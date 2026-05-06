using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;//保存管理器的单例实例，方便其他脚本访问

    private FileDataHandler dataHandle;//用于处理数据文件的读写操作
    private GameData gameData;
    private List<ISaveable> allSaveables;//保存所有实现了ISaveable接口的对象的列表

    [SerializeField] private string fileName = "saveData.json";
    [SerializeField] private bool encryptData = true;

    private void Awake()
    {
        instance = this;//在Awake方法中将当前对象赋值给instance变量，确保单例模式的实现
    }

    private IEnumerator Start()
    {
        Debug.Log(Application.persistentDataPath);//输出数据文件夹路径，方便调试
        dataHandle = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);//创建FileDataHandle对象，传入数据文件夹路径和数据文件名称
        allSaveables = FindISaveables();//查找场景中所有实现了ISaveable接口的对象，并将它们添加到allSaveables列表中

        yield return null;//等待一帧，确保所有对象都已经初始化完成
        LoadGame();//加载游戏数据
    }

    private void LoadGame()
    {
        gameData = dataHandle.LoadData();//调用FileDataHandle对象的LoadData方法，从数据文件中加载游戏数据到gameData对象中

        if (gameData == null)//如果gameData对象为null，说明没有找到数据文件或数据文件为空，则创建一个新的GameData对象
        {
            Debug.Log("No data found, creating new game data.");
            gameData = new GameData();
            // 新档也要让所有 ISaveable 初始化默认值
            foreach (var saveable in allSaveables)
                saveable.LoadData(gameData);
            return;
        }

        foreach (var saveable in allSaveables)//遍历所有实现了ISaveable接口的对象，调用它们的LoadData方法，将gameData对象中的游戏数据加载到这些对象中
            saveable.LoadData(gameData);
    }

    public void SaveGame()//保存游戏数据的方法
    {
        foreach (var saveable in allSaveables)//遍历所有实现了ISaveable接口的对象，调用它们的SaveData方法，将游戏数据保存到gameData对象中
            saveable.SaveData(ref gameData);

        dataHandle.SaveData(gameData);//调用FileDataHandle对象的SaveData方法，将gameData对象保存到数据文件中
    }

    public GameData GetGameData()//获取游戏数据的方法，返回gameData对象
    {
        return gameData;
    }

    [ContextMenu("删除游戏数据")]//在Unity编辑器中添加一个上下文菜单项，点击后会调用DeleteSaveData方法
    public void DeleteSaveData()//删除游戏数据的方法
    {
        dataHandle = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);//重新创建FileDataHandle对象，确保它指向正确的数据文件路径和名称
        dataHandle.Delete();

        LoadGame();//删除数据后重新加载游戏数据，确保游戏状态被重置
    }

    private void OnApplicationQuit()
    {
        SaveGame();//当应用程序退出时，调用SaveGame方法保存游戏数据 
    }

    private List<ISaveable> FindISaveables()//查找场景中所有实现了ISaveable接口的对象，并将它们添加到allSaveables列表中
    {
        //FindObjectsOfType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)方法用于查找场景中所有的MonoBehaviour对象
        //包括未激活的对象。然后使用OfType<ISaveable>()方法筛选出实现了ISaveable接口的对象，并将它们转换为List<ISaveable>类型返回。
        return
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<ISaveable>()
                .ToList();
    }
}
