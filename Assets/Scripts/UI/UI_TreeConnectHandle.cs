using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UI_TreeConnectDetails//单条连接线配置
{
    public UI_TreeConnectHandle childNode;//子节点连接句柄
    public NodeDirectionType direction;//连接方向
    [Range(100f, 350f)] public float length;//连接长度
    [Range(-50f, 50f)] public float rotation;//额外旋转偏移
}

public class UI_TreeConnectHandle : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;//连接线配置列表
    [SerializeField] private UI_TreeConnection[] connections;//连接线组件列表

    private Image connectionImage;
    private Color orginalColor;

    private void Awake()
    {
        if (connectionImage != null)
            orginalColor = connectionImage.color;//缓存连接线初始颜色
    }

    private void OnValidate()//编辑器参数变化时自动刷新连接线
    {
        if (connectionDetails.Length <= 0)//没有配置时直接返回
            return;

        if (connectionDetails.Length != connections.Length)//配置数量与组件数量必须一致
        {
            Debug.LogWarning("连接线细节的数量和连接线的数量不匹配，请确保它们相同");
            return;
        }

        UpdateConnections();
    }

    public void UpdateConnections()
    {
        for (int i = 0; i < connectionDetails.Length; i++)
        {
            var detail = connectionDetails[i];//当前连接配置
            var connection = connections[i];//当前连接组件

            Vector2 targetPosition = connection.GetConnectionPoint(rect);//计算子节点连接点在当前父节点坐标系下的位置
            Image connectionImage = connection.GetConnectionImage();//读取当前连接线的 Image

            connections[i].DirectConnection(detail.direction, detail.length, detail.rotation);//按配置刷新方向、长度和旋转

            if (detail.childNode == null)
                continue;//没有子节点时跳过递归准备

            detail.childNode.SetPosition(targetPosition);//同步子节点位置
            detail.childNode.SetConnectionImage(connectionImage);//传递连接线 Image 引用
           // detail.childNode.transform.SetAsLastSibling();//可选：把连接节点放到层级末尾，避免遮挡
        }
    }

    public void UpdateAllConnections()
    {
        UpdateConnections();//先刷新当前层连接线

        foreach (var node in connectionDetails)
        {
            if (node.childNode == null)
                continue;
            node.childNode?.UpdateConnections();//继续刷新下一层连接线
        }
    }

    public void UnlockConnectionImage(bool unlocked)
    {
        if (connectionImage == null)
            return;
        connectionImage.color = unlocked ? Color.white : orginalColor;//解锁时显示白色，未解锁还原原色
    }

    public void SetConnectionImage(Image image) => connectionImage = image;//设置连接线 Image 引用
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;//设置当前连接节点位置
}
