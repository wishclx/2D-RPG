using UnityEngine;

public class VFX_AutoController : MonoBehaviour
{
    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1f;// 破坏延迟时间，单位为秒
    [Space]
    [SerializeField] private bool randomOffset = true;// 是否随机化位置  
    [SerializeField] private bool randomRotation = true;// 是否随机化旋转
    [Header("Random Rotation")]
    [SerializeField] private float minRotation = 0f;// 最小旋转角度
    [SerializeField] private float maxRotation = 360f;// 最大旋转角度

    [Header("Random Position")]
    [SerializeField] private float xMinOffset = -.3f;// X轴最小偏移量
    [SerializeField] private float xMaxOffset = .3f;// X轴最大偏移量
    [Space]
    [SerializeField] private float yMinOffset = -.3f;// Y轴最小偏移量
    [SerializeField] private float yMaxOffset = .3f;// Y轴最大偏移量

    private void Start()
    {
        ApplyRandomOffset();// 在Start方法中调用ApplyRandomOffset方法来应用随机偏移
        ApplyRandomRotation();// 在Start方法中调用ApplyRandomRotation方法来应用随机旋转

        if (autoDestroy)
            Destroy(gameObject, destroyDelay);// 在指定的延迟时间后销毁游戏对象
    }

    private void ApplyRandomOffset()
    {
        if (!randomOffset)
            return;

        float xOffset = Random.Range(xMinOffset, xMaxOffset);// 生成一个在X轴最小和最大偏移量之间的随机数
        float yOffset = Random.Range(yMinOffset, yMaxOffset);// 生成一个在Y轴最小和最大偏移量之间的随机数
        transform.position += new Vector3(xOffset, yOffset, 0f);// 将随机偏移应用到当前游戏对象的位置上
    }

    private void ApplyRandomRotation()
    {
        if (!randomRotation)
            return;

        float zRotation = Random.Range(minRotation, maxRotation);// 生成一个在最小和最大旋转角度之间的随机数
        transform.Rotate(0, 0, zRotation);// 将随机旋转应用到当前游戏对象的旋转上
    }
}
