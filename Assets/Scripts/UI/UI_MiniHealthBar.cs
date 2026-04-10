using UnityEngine;

public class UI_MiniHealthBar : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();//缓存父对象上的 Entity 引用
    }

    private void OnEnable()
    {
        entity.OnFlipped += HandleFlip;//订阅翻转事件
    }

    private void OnDisable()
    {
        entity.OnFlipped -= HandleFlip;
    }

    private void HandleFlip() => transform.rotation = Quaternion.identity;
    // 实体翻转时重置血条旋转，保持 UI 正向显示。
}
