using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Portal : MonoBehaviour, ISaveable
{
    public static Object_Portal instance;//单例模式，方便其他脚本访问传送门

    public bool isActive { get; private set; }//传送门是否激活，只有激活的传送门才能被玩家使用
    [SerializeField] private Vector2 defaultPosition;//传送门默认位置
    [SerializeField] private string townSceneName = "Level_0";

    [SerializeField] private Transform respawnPoint;//传送门的重生点，如果没有设置则默认使用传送门的位置
    [SerializeField] private bool canBeTriggered;

    private string currentSceneName;
    private string returnSceneName;//返回场景的名字，方便在需要时使用
    private bool returningFromTown;//是否从城镇返回

    private void Awake()
    {
        instance = this;
        currentSceneName = SceneManager.GetActiveScene().name;//获取当前场景的名字
        transform.position = new Vector3(9999, 9999);//默认将传送门放置在一个远离玩家的位置，直到需要使用时再将其移动到正确的位置 
    }

    public void ActivatePortal(Vector3 position, int facingDir = 1)
    {
        isActive = true;
        transform.position = position;//将传送门移动到指定位置，准备被玩家使用

        // SaveManager 可能为空（编辑器/测试场景等），先做保护检查
        if (SaveManager.instance != null && SaveManager.instance.GetGameData() != null)
            SaveManager.instance.GetGameData().inScenePortals.Clear();

        if (facingDir == -1)
            transform.Rotate(0, 180, 0);//如果传送门需要面向相反的方向，则旋转180度
    }

    public void DisableIfNeeded()
    {
        if (returningFromTown == false)
            return;//如果不是从城镇返回的，则不需要禁用传送门，直接返回

        //从游戏数据中移除当前场景的传送门位置，避免在加载游戏时恢复一个不存在的传送门位置
        SaveManager.instance.GetGameData().inScenePortals.Remove(currentSceneName);
        isActive = false;
        transform.position = new Vector3(9999, 9999);//将传送门移动到一个远离玩家的位置，避免玩家在不需要使用传送门时误触发它
    }

    private void UseTeleport()
    {
        //传送到目标场景
        string destinationScene = InTown() ? returnSceneName : townSceneName;//如果当前在城镇场景，则传送到返回场景，否则传送到城镇场景

        GameManager.instance.ChangeScene(destinationScene, RespawnType.Portal);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTriggered == false)
            return;

        UseTeleport();
    }

    private void OnTriggerExit2D(Collider2D collision) => canBeTriggered = true;

    public void SetTrigger(bool trigger) => canBeTriggered = trigger;

    //获取传送门位置，如果respawnPoint存在则返回respawnPoint的位置，否则返回传送门自身的位置
    public Vector3 GetPostion() => respawnPoint != null ? respawnPoint.position : transform.position;

    private bool InTown() => currentSceneName == townSceneName;//判断当前场景是否是城镇场景

    public void LoadData(GameData data)
    {
        if (string.IsNullOrEmpty(currentSceneName))
            currentSceneName = SceneManager.GetActiveScene().name;//删除存档后可能尚未初始化场景名

        if (InTown() && data.inScenePortals.Count > 0)//如果当前场景是城镇场景，并且游戏数据中有传送门位置的数据，则将传送门移动到保存的位置，并激活传送门
        {
            transform.position = defaultPosition;//将传送门移动到默认位置
            isActive = true;//激活传送门，使其可以被玩家使用
        }
        else if (data.inScenePortals.TryGetValue(currentSceneName, out Vector3 portalPosition))
        {
            transform.position = portalPosition;//将传送门移动到保存的位置
            isActive = true;//激活传送门，使其可以被玩家使用
        }

        returningFromTown = data.returningFromTown;//从游戏数据中获取是否从城镇返回的信息，方便在需要时使用
        returnSceneName = data.portalDestinationSceneName;//从游戏数据中获取返回场景的名字，方便在需要时使用
    }

    public void SaveData(ref GameData data)
    {
        data.returningFromTown = InTown();

        //如果传送门激活，并且当前场景不是城镇场景，则将当前场景的名字保存到游戏数据中，作为传送门的目的地场景，方便在加载游戏时恢复传送门的位置
        if (isActive && InTown() == false)
        {
            //如果传送门激活，则将当前场景的名字保存到游戏数据中，作为传送门的目的地场景，方便在加载游戏时恢复传送门的位置
            data.portalDestinationSceneName = currentSceneName;
            data.inScenePortals[currentSceneName] = transform.position;//将当前场景中传送门的位置保存到游戏数据中，方便在加载游戏时恢复传送门的位置
        }
        else
        {
            //如果传送门没有激活，则从游戏数据中移除当前场景的传送门位置，避免在加载游戏时恢复一个不存在的传送门位置
            data.inScenePortals.Remove(currentSceneName);
        }

    }
}
