using UnityEngine;

public class UI_MiniHealthBar : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void OnEnable()
    {
        entity.OnFlipped += HandleFlip;// 订阅翻转事件，以便在实体翻转时调整血条的旋转。
    }

    private void OnDisable()
    {
        entity.OnFlipped -= HandleFlip;
    }

    private void HandleFlip() => transform.rotation = Quaternion.identity;

}


