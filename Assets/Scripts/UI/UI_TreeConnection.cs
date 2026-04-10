using UnityEngine;
using UnityEngine.UI;

public class UI_TreeConnection : MonoBehaviour
{
    [SerializeField] private RectTransform rotationPoint;
    [SerializeField] private RectTransform connectionLength;//控制连接线长度的节点
    [SerializeField] private RectTransform childNodeConnectionPoint;//子节点连接锚点

    public void DirectConnection(NodeDirectionType direction, float length, float offset)
    {
        bool shouldBeActive = direction != NodeDirectionType.None;//方向不是 None 时才显示连接线
        float finalLength = shouldBeActive ? length : 0f;//未启用时长度强制为 0
        float angle = GetDirectionAngle(direction);//把方向转换为角度

        rotationPoint.localRotation = Quaternion.Euler(0f, 0f, angle + offset);//应用朝向与额外偏移
        connectionLength.sizeDelta = new Vector2(finalLength, connectionLength.sizeDelta.y);//更新连接线长度
    }

    public Image GetConnectionImage() => connectionLength.GetComponent<Image>();//获取连接线 Image 组件

    public Vector2 GetConnectionPoint(RectTransform rect)//计算连接点在父节点坐标系下的位置
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle//将子节点锚点的世界坐标转换为父节点局部坐标
            (
            rect.parent as RectTransform,
            childNodeConnectionPoint.position,
            null,
            out var localPosition
            );

        return localPosition;
    }

    private float GetDirectionAngle(NodeDirectionType type)//按方向返回对应角度
    {
        switch (type)
        {
            case NodeDirectionType.UpLeft: return 135f;
            case NodeDirectionType.Up: return 90f;
            case NodeDirectionType.UpRight: return 45f;
            case NodeDirectionType.Left: return 180f;
            case NodeDirectionType.Right: return 0f;
            case NodeDirectionType.DownLeft: return -135f;
            case NodeDirectionType.Down: return -90f;
            case NodeDirectionType.DownRight: return -45f;
            default: return 0f;
        }
    }

}

public enum NodeDirectionType
{
    None,
    UpLeft,
    Up,
    UpRight,
    Left,
    Right,
    DownLeft,
    Down,
    DownRight
}
