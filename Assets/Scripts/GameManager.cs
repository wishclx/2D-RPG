using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;
    private Vector3 lastPlayerPosition;//玩家上次死亡的位置

    private string lastScenePlayed;//玩家上次进入的场景名称
    private bool dataLoaded;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);//如果已经存在实例，销毁当前对象
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);//保证场景切换时不销毁该对象
    }

    //public void SetLastPlayerPosition(Vector3 position) => lastPlayerPosition = position;//设置玩家上次死亡的位置

    public void ContinuePlay()
    {
        ChangeScene(lastScenePlayed, RespawnType.NoneSpecific);//继续游戏，切换到玩家上次进入的场景，并根据玩家上次死亡的位置来选择新的出生点
    }

    public void RestartScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;//获取当前场景的名称
        ChangeScene(sceneName, RespawnType.NoneSpecific);//重新加载当前场景，并根据玩家上次死亡的位置来选择新的出生点
    }

    public void ChangeScene(string sceneName, RespawnType spawnType)
    {
        SaveManager.instance.SaveGame();//保存游戏数据

        Time.timeScale = 1.0f;//确保时间缩放恢复正常，防止在暂停状态下切换场景后继续保持暂停状态
        StartCoroutine(ChangeSceneCo(sceneName, spawnType));// 开始切换场景的协程
    }

    private IEnumerator ChangeSceneCo(string sceneName, RespawnType spawnType)
    {
        UI_FadeScreen fadeScreen = FindFadeScreenUI();// 查找淡入淡出界面

        //TODO: 添加过渡动画或加载界面
        fadeScreen.DoFadeOut();// 透明 -> 全黑
        yield return fadeScreen.fadeEffectCo;// 等待淡入动画完成

        SceneManager.LoadScene(sceneName);

        dataLoaded = false;// 切换场景后重置数据加载状态
        yield return null;// 等待一帧，确保场景切换完成

        while (dataLoaded == false)
        {
            yield return null;// 等待游戏数据加载完成
        }

        fadeScreen = FindFadeScreenUI();// 场景切换后重新查找淡入淡出界面
        fadeScreen.DoFadeIn();// 全黑 -> 透明

        Player player = Player.instance;// 获取玩家实例

        if (player == null)
            yield break;// 如果玩家实例不存在，退出协程

        Vector3 position = GetNewPlayerPosition(spawnType);// 获取新的玩家位置

        if (position != Vector3.zero)
            player.TeleportPlayer(position);// 将玩家传送到指定位置
    }

    private UI_FadeScreen FindFadeScreenUI()
    {
        if (UI.instance != null)
            return UI.instance.fadeScreenUI;// 从UI单例中获取淡入淡出界面
        else
            return FindFirstObjectByType<UI_FadeScreen>();// 如果UI单例不存在，直接在场景中查找淡入淡出界面
    }

    private Vector3 GetNewPlayerPosition(RespawnType type)
    {

        if (type == RespawnType.Portal)
        {
            Object_Portal portal = Object_Portal.instance;// 获取传送门实例

            Vector3 position = portal.GetPostion();// 获取传送门的位置

            portal.SetTrigger(false);// 禁用传送门的触发器，防止玩家在切换场景后立即触发传送门
            portal.DisableIfNeeded();

            return position;
        }

        if (type == RespawnType.NoneSpecific)// 如果传入的类型是None，表示需要根据玩家上次死亡的位置来选择新的出生点
        {
            var data = SaveManager.instance.GetGameData();// 获取游戏数据
            var checkpoint = FindObjectsByType<Object_Checkpoint>(FindObjectsSortMode.None);// 获取所有检查点对象
                                                                                            // 获取所有已解锁的检查点位置
            var unlockedCheckpoints = checkpoint
                .Where(cp => data.unlockedCheckpoints.TryGetValue(cp.GetCheckpointId(), out bool isUnlocked) && isUnlocked)
                .Select(cp => cp.GetPosition())
                .ToList();

            // 获取所有分界点对象
            // 可注释,注释后玩家将只能从检查点复活,无法从分界点复活
            var enterWaypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None)
                .Where(wp => wp.GetWaypointType() == RespawnType.Enter)
                .Select(wp => wp.GetPositionAndSetTriggerFalse())
                .ToList();

            var selectedPosition = unlockedCheckpoints.Concat(enterWaypoints).ToList();// 将解锁的检查点位置和分界点位置合并成一个列表

            if (selectedPosition.Count == 0)
                return Vector3.zero;// 如果没有可用的位置，返回零向量

            return selectedPosition
                .OrderBy(pos => Vector3.Distance(pos, lastPlayerPosition))// 按照与玩家上次死亡位置的距离排序
                .First();// 返回距离玩家上次死亡位置最近的位置
        }

        return GetWaypointPosition(type);
    }

    private Vector3 GetWaypointPosition(RespawnType type)
    {
        var waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);

        foreach (var point in waypoints)
        {
            if (point.GetWaypointType() == type)
                return point.GetPositionAndSetTriggerFalse();
        }

        return Vector3.zero;
    }

    public void LoadData(GameData data)
    {
        lastScenePlayed = data.lastScenePlayed;// 从游戏数据中加载玩家上次进入的场景名称
        lastPlayerPosition = data.lastPlayerPosition;

        if (string.IsNullOrEmpty(lastScenePlayed))
            lastScenePlayed = "关卡1";// 如果没有保存的场景名称，默认设置为Level_0

        dataLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
            return;// 如果当前场景是主菜单，不保存上次进入的场景名称

        data.lastPlayerPosition = Player.instance.transform.position;// 将玩家当前的位置保存到游戏数据中
        data.lastScenePlayed = currentScene;// 将当前场景名称保存到游戏数据中
        dataLoaded = false;
    }
}
