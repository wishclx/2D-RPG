using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UI_TreeConnectDetails//连接线细节
{
    public UI_TreeConnectHandle childNode;//子节点连接点
    public NodeDirectionType direction;//连接线方向
    [Range(100f, 350f)] public float length;//连接线长度
    [Range(-50f, 50f)] public float rotation;//连接线旋转角度
}

public class UI_TreeConnectHandle : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;//连接线细节
    [SerializeField] private UI_TreeConnection[] connections;//连接线

    private Image connectionImage;
    private Color orginalColor;

    private void Awake()
    {
        if (connectionImage != null)
            orginalColor = connectionImage.color;//保存连接线的原始颜色
    }

    private void OnValidate()//在编辑器中修改连接线细节时自动更新连接线
    {
        if (connectionDetails.Length <= 0)//如果连接线细节的数量小于等于0，那么就不需要更新连接线，直接返回
            return;

        if (connectionDetails.Length != connections.Length)//如果连接线细节的数量和连接线的数量不匹配，那么就输出警告信息
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
            var detail = connectionDetails[i];//获取连接线细节
            var connection = connections[i];//获取连接线

            Vector2 targetPosition = connection.GetConnectionPoint(rect);//获取连接点在父节点坐标系中的位置 
            Image connectionImage = connection.GetConnectionImage();//获取连接线的Image组件

            connections[i].DirectConnection(detail.direction, detail.length, detail.rotation);//根据连接线细节更新连接线

            if (detail.childNode == null)
                continue;//如果子节点连接点为null，那么就跳过设置连接点位置和连接线Image组件的步骤

            detail.childNode.SetPosition(targetPosition);//设置连接点的位置
            detail.childNode.SetConnectionImage(connectionImage);//设置连接线的Image组件
            detail.childNode.transform.SetAsLastSibling();//将子节点连接点设置为父节点的最后一个子对象，以确保连接线在所有节点的下方显示
        }
    }

    public void UpdateAllConnections()
    {
        UpdateConnections();//更新连接线

        foreach (var node in connectionDetails)
        {
            if (node.childNode == null)
                continue;
            node.childNode?.UpdateConnections();//递归更新所有子节点连接线
        }
    }

    public void UnlockConnectionImage(bool unlocked)
    {
        if (connectionImage == null)
            return;
        connectionImage.color = unlocked ? Color.white : orginalColor;//如果连接线解锁了，那么就将连接线的颜色设置为白色，否则设置为原始颜色
    }

    public void SetConnectionImage(Image image) => connectionImage = image;//设置连接线的Image组件
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;//设置连接点的位置
}
