using UnityEngine;

public class UI_DeathScreen : MonoBehaviour
{
    public void GoToCampBTN()
    {
        GameManager.instance.ChangeScene("Level_0", RespawnType.NoneSpecific);//切换到营地场景，并根据玩家上次死亡的位置来选择新的出生点
    }

    public void GoToCheckpointBTN()
    {
        GameManager.instance.RestartScene();//重新加载当前场景，并根据玩家上次死亡的位置来选择新的出生点
    }

    public void GoToMainMenuBTN()
    {
        GameManager.instance.ChangeScene("MainMenu", RespawnType.NoneSpecific);//切换到主菜单场景
    }
}
