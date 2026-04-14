using UnityEngine;

/// <summary>
/// UI_ToolTip 的职责说明。
/// </summary>
public class UI_ToolTip : MonoBehaviour
{
    private RectTransform rect; // 当前提示框的矩形组件。
    [SerializeField] private Vector2 offset = new Vector2(300, 20); // 相对目标 UI 的显示偏移量。

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 显示或隐藏提示框。
    /// 显示时会根据目标 UI 位置自动调整提示框位置。
    /// </summary>
    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        gameObject.SetActive(show);
        if (show)
        {
            UpdatePosition(targetRect);
        }
    }

    /// <summary>
    /// 执行 UpdatePosition 逻辑。
    /// </summary>
    private void UpdatePosition(RectTransform targetRect)
    {
        // 屏幕边界与中心线，用于决定提示框显示方向与上下边界限制。
        float screenCenterX = Screen.width / 2f;
        float screenTop = Screen.height;
        float screenBottom = 0;

        // 以目标 UI 位置作为提示框定位起点。
        Vector2 targetPosistion = targetRect.position;

        // 根据目标点位于屏幕左/右半区，决定提示框向左或向右偏移。
        targetPosistion.x = targetPosistion.x > screenCenterX ? targetPosistion.x - offset.x : targetPosistion.x + offset.x;

        float veritcalHalf = rect.sizeDelta.y / 2;
        float toolY = targetPosistion.y + veritcalHalf;
        float bottomY = targetPosistion.y - veritcalHalf;

        // 防止提示框超出屏幕上边界或下边界。
        if (toolY > screenTop)
        {
            targetPosistion.y = screenTop - veritcalHalf - offset.y;
        }
        else if (bottomY < screenBottom)
        {
            targetPosistion.y = screenBottom + veritcalHalf + offset.y;
        }


        rect.position = targetPosistion;
    }

    /// <summary>
    /// 执行 GetColoredText 逻辑。
    /// </summary>
    protected string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }
}


