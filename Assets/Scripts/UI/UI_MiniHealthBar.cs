using UnityEngine;

public class UI_MiniHealthBar : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();// 在Awake方法中获取父对象上的Entity组件的引用，并将其存储在entity变量中
    }

    private void OnEnable()
    {
        entity.OnFlipped += HandleFlip;// 订阅Entity的OnFlipped事件，当实体翻转时调用HandleFlip方法
    }

    private void OnDisable()
    {
        entity.OnFlipped -= HandleFlip;
    }

    private void HandleFlip() => transform.rotation = Quaternion.identity;
    // 在每一帧更新中，将UI元素的旋转设置为默认值（无旋转），以确保它始终面向玩家或摄像机。
}
