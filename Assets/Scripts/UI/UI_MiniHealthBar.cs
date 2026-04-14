using UnityEngine;

/// <summary>
/// UI_MiniHealthBar 的职责说明。
/// </summary>
public class UI_MiniHealthBar : MonoBehaviour
{
    private Entity entity;

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    /// <summary>
    /// 执行 OnEnable 逻辑。
    /// </summary>
    private void OnEnable()
    {
        entity.OnFlipped += HandleFlip;// 订阅翻转事件，以便在实体翻转时调整血条的旋转。
    }

    /// <summary>
    /// 执行 OnDisable 逻辑。
    /// </summary>
    private void OnDisable()
    {
        entity.OnFlipped -= HandleFlip;
    }

    private void HandleFlip() => transform.rotation = Quaternion.identity;

}


