using UnityEngine;

public class Object_Checkpoint : MonoBehaviour, ISaveable
{
    [SerializeField] private string checkpointId;
    [SerializeField] private Transform respawnPoint;

    public bool isActive { get; private set; }
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public string GetCheckpointId() => checkpointId;

    public Vector3 GetPosition() => respawnPoint == null ? transform.position : respawnPoint.position;//获取重生点位置，如果没有设置重生点，则返回检查点的位置

    public void ActivateCheckpoint(bool activate)
    {
        isActive = activate;
        anim.SetBool("isActivate", activate);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //保存
        ActivateCheckpoint(true);//激活当前检查点
    }

    public void LoadData(GameData data)
    {
        bool active = data.unlockedCheckpoints.TryGetValue(checkpointId, out active);//从数据中获取当前检查点的激活状态
        ActivateCheckpoint(active);//根据数据中的激活状态来激活当前检查点
    }

    public void SaveData(ref GameData data)
    {
        if (isActive == false)
            return;

        if (data.unlockedCheckpoints.ContainsKey(checkpointId) == false)//如果数据中没有当前检查点的ID，则添加到数据中
            data.unlockedCheckpoints.Add(checkpointId, true);//将当前检查点的ID添加到数据中，并标记为已解锁
    }

    private void OnValidate()
    {
# if UNITY_EDITOR
        if (string.IsNullOrEmpty(checkpointId))//如果检查点ID为空，则生成一个新的唯一ID
        {
            checkpointId = System.Guid.NewGuid().ToString();
        }
#endif
    }

}
