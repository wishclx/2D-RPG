using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
/// <summary>
/// UI_TreeConnectDetails 的职责说明。
/// </summary>
public class UI_TreeConnectDetails
{
    public UI_TreeConnectHandle childNode;//子节点连接句柄
    public NodeDirectionType direction;
    [Range(100f, 350f)] public float length;
    [Range(-50f, 50f)] public float rotation;//额外旋转偏移
}

/// <summary>
/// UI_TreeConnectHandle 的职责说明。
/// </summary>
public class UI_TreeConnectHandle : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;
    [SerializeField] private UI_TreeConnection[] connections;

    private Image connectionImage;
    private Color orginalColor;

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    private void Awake()
    {
        if (connectionImage != null)
            orginalColor = connectionImage.color;
    }

    public UI_TreeNode[] GetChildNodes()//获取所有子节点的 UI_TreeNode 组件
    {
        List<UI_TreeNode> childToReturn = new List<UI_TreeNode>();

        foreach (var node in connectionDetails)
        {
            if (node.childNode != null)
                childToReturn.Add(node.childNode.GetComponent<UI_TreeNode>());
        }

        return childToReturn.ToArray();
    }

    /// <summary>
    /// 执行 UpdateConnections 逻辑。
    /// </summary>
    public void UpdateConnections()
    {
        for (int i = 0; i < connectionDetails.Length; i++)
        {
            var detail = connectionDetails[i];
            var connection = connections[i];

            Vector2 targetPosition = connection.GetConnectionPoint(rect);
            Image connectionImage = connection.GetConnectionImage();

            connections[i].DirectConnection(detail.direction, detail.length, detail.rotation);

            if (detail.childNode == null)
                continue;//没有子节点时跳过递归处理

            detail.childNode.SetPosition(targetPosition);
            detail.childNode.SetConnectionImage(connectionImage);
            detail.childNode.transform.SetAsLastSibling();//确保子节点在连接线之上显示
        }
    }

    /// <summary>
    /// 执行 UpdateAllConnections 逻辑。
    /// </summary>
    public void UpdateAllConnections()
    {
        UpdateConnections();

        foreach (var node in connectionDetails)
        {
            if (node.childNode == null)
                continue;
            node.childNode?.UpdateConnections();
        }
    }

    /// <summary>
    /// 执行 UnlockConnectionImage 逻辑。
    /// </summary>
    public void UnlockConnectionImage(bool unlocked)
    {
        if (connectionImage == null)
            return;
        connectionImage.color = unlocked ? Color.white : orginalColor;//解锁时显示白色，未解锁时恢复原色
    }

    public void SetConnectionImage(Image image) => connectionImage = image;
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;

    private void OnValidate()
    {
        if (connectionDetails.Length <= 0)//没有配置时直接返回
            return;

        if (connectionDetails.Length != connections.Length)
        {
            Debug.LogWarning("连接细节数量与连接线数量不匹配，请保持一致");
            return;
        }

        UpdateConnections();
    }
}



