using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private Vector2 offset = new Vector2(300, 20);//工具提示相对于目标矩形的偏移量

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public virtual void ShowToolTip(bool show, RectTransform targetRect)//显示或隐藏工具提示
    {
        gameObject.SetActive(show);//根据show参数设置工具提示的激活状态
        if (show)
        {
            UpdatePosition(targetRect);
        }
    }

    private void UpdatePosition(RectTransform targetRect)
    {
        float screenCenterX = Screen.width / 2f;//屏幕中心的X坐标
        float screenTop = Screen.height;//屏幕顶部的Y坐标
        float screenBottom = 0;

        Vector2 targetPosistion = targetRect.position;//目标矩形的位置

        targetPosistion.x = targetPosistion.x > screenCenterX ? targetPosistion.x - offset.x : targetPosistion.x + offset.x;//根据目标矩形的位置调整工具提示的X坐标，使其不会遮挡目标矩形

        float veritcalHalf = rect.sizeDelta.y / 2;//根据工具提示的高度调整工具提示的Y坐标，使其不会超出屏幕顶部或底部
        float toolY = targetPosistion.y + veritcalHalf;//工具提示的Y坐标
        float bottomY = targetPosistion.y - veritcalHalf;//工具提示的底部Y坐标

        if (toolY > screenTop)
        {
            targetPosistion.y = screenTop - veritcalHalf - offset.y;//如果工具提示超出屏幕顶部，则将其调整到屏幕顶部
        }
        else if (bottomY < screenBottom)
        {
            targetPosistion.y = screenBottom + veritcalHalf + offset.y;//如果工具提示超出屏幕底部，则将其调整到屏幕底部
        }


        rect.position = targetPosistion;//更新工具提示的位置
    }

    protected string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text}</color>";//将文本包装在颜色标签中，以便在UI中显示带有颜色的文本
    }
}
