using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped;// 定义一个事件，当实体翻转时触发

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Entity_Stats stats { get; private set; }
    protected StateMachine stateMachine;


    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;

    [Header("Collision detection")]
    [SerializeField] protected LayerMask whatIsGround;//地面图层
    [SerializeField] private float groundCheckDistance;//检测地面距离
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;//检测地面的点
    [SerializeField] private Transform primaryWallCheck;// 检测墙壁的点
    [SerializeField] private Transform secondaryWallCheck;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }

    //Knockback
    private bool isKnocked;
    private Coroutine knockbackCo;// 击退协程
    private Coroutine slowDownCo;// 减速协程

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Entity_Stats>();

        stateMachine = new StateMachine();

    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public virtual void EntityDeath()
    {

    }

    public virtual void SlowDownEntity(float duration, float slowMultiplier)
    {
        if (slowDownCo != null)
            StopCoroutine(slowDownCo);

        slowDownCo = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier));// 启动减速协程
    }

    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)// 定义一个协程来处理实体的减速效果
    {
        yield return null;
    }

    public void ReciveKnockback(Vector2 knockback, float duration)
    {
        if (knockbackCo != null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));// 启动击退协程
    }

    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public void SetVelocity(float xvelocity, float yvelocity)
    {
        if (isKnocked)
            return;

        rb.linearVelocity = new Vector2(xvelocity, yvelocity);
        HandleFlip(xvelocity);
    }

    public void HandleFlip(float xVelocity)
    {
        if ((xVelocity > 0 && !facingRight) || (xVelocity < 0 && facingRight))
            Flip();
    }

    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        facingDir *= -1;

        OnFlipped?.Invoke();// 触发翻转事件，通知订阅者实体已经翻转
    }

    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);//检测地面

        if (secondaryWallCheck != null)
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
            && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }
        else
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        if (secondaryWallCheck != null)
            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
    }
}
