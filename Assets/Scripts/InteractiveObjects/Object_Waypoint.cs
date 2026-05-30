using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Waypoint : MonoBehaviour
{

    [SerializeField] private string transferToScene;//要前往的场景名称
    [Space]
    [SerializeField] private RespawnType watpointType;//分界点类型
    [SerializeField] private RespawnType conntedWaypoint;
    [SerializeField] private Transform respwanPoint;//出生点位置
    [SerializeField] private bool canBeTriggered = true;

    public RespawnType GetWaypointType() => watpointType;

    public Vector3 GetPositionAndSetTriggerFalse()
    {
        canBeTriggered = false;//进入分界点后暂时无法再次触发，直到玩家离开分界点
        return respwanPoint == null ? transform.position : respwanPoint.position;
    }

    private void OnValidate()
    {
        gameObject.name = "分界点 - " + watpointType.ToString() + " - " + transferToScene;

        //根据分界点类型设置连接的分界点类型
        if (watpointType == RespawnType.Enter)
            conntedWaypoint = RespawnType.Exit;

        if (watpointType == RespawnType.Exit)
            conntedWaypoint = RespawnType.Enter;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == false)
            return;

        if (canBeTriggered == false)
            return;

        //前往下一场景
        GameManager.instance.ChangeScene(transferToScene, conntedWaypoint);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;//离开分界点后可以再次触发
    }
}
