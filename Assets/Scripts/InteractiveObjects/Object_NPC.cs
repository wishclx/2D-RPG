using UnityEngine;

public class Object_NPC : MonoBehaviour
{
    protected Transform player;//玩家位置
    protected UI ui;//UI界面

    [SerializeField] Transform npc;
    [SerializeField] private GameObject interactToolTip;//交互提示UI
    private bool facingRight = true;//NPC默认朝右

    [Header("Floaty ToolTip")]
    [SerializeField] private float floatSpeed = 8f;
    [SerializeField] private float floatRange = .1f;
    private Vector3 statrPosistion;

    protected virtual void Awake()
    {
        ui = FindFirstObjectByType<UI>();//获取UI界面
        statrPosistion = interactToolTip.transform.position;//记录交互提示UI的初始位置
        interactToolTip.SetActive(false);//初始时交互提示UI处于关闭状态
    }

    protected virtual void Update()
    {
        HandleNpcFlip();
        HandleToolTipFloat();
    }

    private void HandleToolTipFloat()
    {
        if (interactToolTip.activeSelf)//如果交互提示UI处于激活状态，则使其浮动
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            interactToolTip.transform.position = statrPosistion + new Vector3(0, yOffset);
        }
    }

    private void HandleNpcFlip()
    {
        if (player == null || npc == null)
            return;

        if (npc.position.x > player.position.x && facingRight)
        {
            npc.transform.Rotate(0, 180, 0);//NPC转向
            facingRight = false;
        }
        else if (npc.position.x < player.position.x && !facingRight)
        {
            npc.transform.Rotate(0, 180, 0);//NPC转向
            facingRight = true;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.transform;//获取玩家位置
        interactToolTip.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        interactToolTip.SetActive(false);//关闭交互提示UI
    }
}
