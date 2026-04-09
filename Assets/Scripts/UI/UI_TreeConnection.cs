using UnityEngine;
using UnityEngine.UI;

public class UI_TreeConnection : MonoBehaviour
{
    [SerializeField] private RectTransform rotationPoint;
    [SerializeField] private RectTransform connectionLength;//连接线长度
    [SerializeField] private RectTransform childNodeConnectionPoint;//子节点连接点

    public void DirectConnection(NodeDirectionType direction, float length, float offset)
    {
        bool shouldBeActive = direction != NodeDirectionType.None;//如果连接线方向不是None，那么就应该激活连接线
        float finalLength = shouldBeActive ? length : 0f;//如果连接线应该被激活，那么就使用传入的长度，否则长度为0
        float angle = GetDirectionAngle(direction);//根据连接线方向类型返回对应的角度

        rotationPoint.localRotation = Quaternion.Euler(0f, 0f, angle + offset);//设置连接点的旋转
        connectionLength.sizeDelta = new Vector2(finalLength, connectionLength.sizeDelta.y);//设置连接线的长度
    }

    public Image GetConnectionImage() => connectionLength.GetComponent<Image>();//获取连接线的Image组件

    public Vector2 GetConnectionPoint(RectTransform rect)//获取连接点在父节点坐标系中的位置
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle//将子节点连接点的世界坐标转换为父节点坐标系中的局部坐标
            (
            rect.parent as RectTransform,
            childNodeConnectionPoint.position,
            null,
            out var localPosition
            );

        return localPosition;
    }

    private float GetDirectionAngle(NodeDirectionType type)//根据连接线方向类型返回对应的角度
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
