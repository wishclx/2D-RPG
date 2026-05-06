using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Portal Scroll", fileName = "Item effect Data - PortalScroll")]
public class ItemEffect_PortalScroll : ItemEffect_DataSO
{
    public override void ExecuteEffect()
    {
        if (SceneManager.GetActiveScene().name == "Level_0")
        {
            Debug.Log("无法在城镇打开传送门");
            return;
        }

        Player player = Player.instance;
        Vector3 portalPosition = player.transform.position + new Vector3(player.facingDir * 1.5f, 0); // 在玩家前方生成传送门  

        Object_Portal.instance.ActivatePortal(portalPosition, player.facingDir);
    }
}
