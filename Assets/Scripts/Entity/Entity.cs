using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Entity 的职责说明。
/// </summary>
public class Entity : MonoBehaviour
{
    public event Action OnFlipped;

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Entity_Stats stats { get; private set; }
    protected StateMachine stateMachine;


    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;

    [Header("Collision detection")]
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }

    // Knockback
    private bool isKnocked;
    private Coroutine knockbackCo;
    private Coroutine slowDownCo;

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Entity_Stats>();

        stateMachine = new StateMachine();

    }

    /// <summary>
    /// 执行 Start 逻辑。
    /// </summary>
    protected virtual void Start()
    {

    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    /// <summary>
    /// 执行 EntityDeath 逻辑。
    /// </summary>
    public virtual void EntityDeath()
    {

    }

    /// <summary>
    /// 执行 SlowDownEntity 逻辑。
    /// </summary>
    public virtual void SlowDownEntity(float duration, float slowMultiplier)
    {
        if (slowDownCo != null)
            StopCoroutine(slowDownCo);

        slowDownCo = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier));
    }

    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        yield return null;
    }

    /// <summary>
    /// 执行 ReciveKnockback 逻辑。
    /// </summary>
    public void ReciveKnockback(Vector2 knockback, float duration)
    {
        if (knockbackCo != null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));
    }

    /// <summary>
    /// 执行 KnockbackCo 逻辑。
    /// </summary>
    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }

    /// <summary>
    /// 执行 CurrentStateAnimationTrigger 逻辑。
    /// </summary>
    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    /// <summary>
    /// 执行 SetVelocity 逻辑。
    /// </summary>
    public void SetVelocity(float xvelocity, float yvelocity)
    {
        if (isKnocked)
            return;

        rb.linearVelocity = new Vector2(xvelocity, yvelocity);
        HandleFlip(xvelocity);
    }

    /// <summary>
    /// 执行 HandleFlip 逻辑。
    /// </summary>
    public void HandleFlip(float xVelocity)
    {
        if ((xVelocity > 0 && !facingRight) || (xVelocity < 0 && facingRight))
            Flip();
    }

    /// <summary>
    /// 执行 Flip 逻辑。
    /// </summary>
    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        facingDir *= -1;

        OnFlipped?.Invoke();
    }

    /// <summary>
    /// 执行 HandleCollisionDetection 逻辑。
    /// </summary>
    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

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

    /// <summary>
    /// 执行 OnDrawGizmos 逻辑。
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        if (secondaryWallCheck != null)
            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
    }
}


