using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private Vector2 offset = new Vector2(300, 20);//提示框相对目标的偏移量

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public virtual void ShowToolTip(bool show, RectTransform targetRect)//显示或隐藏提示框
    {
        gameObject.SetActive(show);//按 show 参数切换显示状态
        if (show)
        {
            UpdatePosition(targetRect);
        }
    }

    private void UpdatePosition(RectTransform targetRect)
    {
        float screenCenterX = Screen.width / 2f;//屏幕中心 X 坐标
        float screenTop = Screen.height;//屏幕顶部 Y 坐标
        float screenBottom = 0;

        Vector2 targetPosistion = targetRect.position;//目标节点的屏幕位置

        targetPosistion.x = targetPosistion.x > screenCenterX ? targetPosistion.x - offset.x : targetPosistion.x + offset.x;//根据左右半屏决定提示框出现方向

        float veritcalHalf = rect.sizeDelta.y / 2;//提示框高度的一半
        float toolY = targetPosistion.y + veritcalHalf;//提示框顶部 Y
        float bottomY = targetPosistion.y - veritcalHalf;//提示框底部 Y

        if (toolY > screenTop)
        {
            targetPosistion.y = screenTop - veritcalHalf - offset.y;//超出顶部则向下回退
        }
        else if (bottomY < screenBottom)
        {
            targetPosistion.y = screenBottom + veritcalHalf + offset.y;//超出底部则向上回退
        }


        rect.position = targetPosistion;//应用最终位置
    }

    protected string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text}</color>";//返回带颜色标签的文本
    }
}
