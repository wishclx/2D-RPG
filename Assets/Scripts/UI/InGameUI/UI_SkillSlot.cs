using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;
    private Image skillIcon;
    private RectTransform rect;
    private Button button;

    private SkillDataSO skillData;//技能数据

    public SkillType skillType;//技能类型
    [SerializeField] private Image cooldownImage;
    [SerializeField] private string inputKeyName;
    [SerializeField] private TextMeshProUGUI inputKeyText;//显示技能对应的输入键
    [SerializeField] private GameObject conflictSlot;//当技能冲突时显示的UI元素

    [SerializeField] private Sprite defaultIconSprite;//默认图标

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        skillIcon = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        button = GetComponent<Button>();
    }

    private void OnValidate()
    {
        gameObject.name = "UI_SkillSlot_" + skillType.ToString();//在编辑器中根据技能类型自动命名游戏对象
    }

    public void SetupSkillSlot(SkillDataSO selectedSkill)
    {
        this.skillData = selectedSkill;

        Color color = Color.black;
        color.a = .6f;
        cooldownImage.color = color;//设置冷却图像的颜色为半透明黑色

        inputKeyText.text = inputKeyName;//显示技能对应的输入键
        skillIcon.sprite = selectedSkill.icon;//设置技能图标

        if (conflictSlot != null)
            conflictSlot.SetActive(false);//如果存在冲突槽，隐藏它
    }

    public void StartCooldown(float cooldown)//开始技能冷却
    {
        cooldownImage.fillAmount = 1;//将冷却图像的填充量设置为1，表示技能刚使用完，处于完全冷却状态
        StartCoroutine(CooldownCo(cooldown));//启动冷却协程，传入技能的冷却时间
    }

    public void ResetCooldown() => cooldownImage.fillAmount = 0;//重置冷却，将填充量设置为0，表示技能可以立即使用

    private IEnumerator CooldownCo(float duraion)//技能冷却协程
    {
        float timePassed = 0;

        while (timePassed < duraion)
        {
            timePassed += Time.deltaTime;
            cooldownImage.fillAmount = 1f - (timePassed / duraion);//根据时间进度更新冷却图像的填充量
            yield return null; //等待下一帧继续执行
        }

        cooldownImage.fillAmount = 0;//冷却结束后将填充量重置为0
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, null);//当鼠标指针离开技能槽时，隐藏技能提示信息
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillData == null)
            return;

        ui.skillToolTip.ShowToolTip(true, rect, skillData, null);//当鼠标指针进入技能槽时，显示技能提示信息，传入技能数据和技能槽的RectTransform
    }

    public void ResetToDefaultVisual()//重置为默认图标与默认UI状态
    {
        if (skillIcon == null)
            skillIcon = GetComponent<Image>();

        if (cooldownImage == null)
            cooldownImage = GetComponentInChildren<Image>(true);

        StopAllCoroutines();
        skillData = null;

        if (skillIcon != null)
            skillIcon.sprite = defaultIconSprite;

        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f;
            cooldownImage.color = new Color(0, 0, 0, 0.6f);
        }

        if (conflictSlot != null)
            conflictSlot.SetActive(false);//重置后不显示冲突遮罩
    }
}
