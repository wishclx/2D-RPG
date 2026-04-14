using System.Collections;
using UnityEngine;

/// <summary>
/// VFX_AutoController 的职责说明。
/// </summary>
public class VFX_AutoController : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1f;
    [Space]
    [SerializeField] private bool randomOffset = true;
    [SerializeField] private bool randomRotation = true;
    [Header("Fade effect")]
    [SerializeField] private bool canFade;
    [SerializeField] private float fadeSpeed = 1f;

    [Header("Random Rotation")]
    [SerializeField] private float minRotation = 0f;
    [SerializeField] private float maxRotation = 360f;

    [Header("Random Position")]
    [SerializeField] private float xMinOffset = -.3f;
    [SerializeField] private float xMaxOffset = .3f;
    [Space]
    [SerializeField] private float yMinOffset = -.3f;
    [SerializeField] private float yMaxOffset = .3f;

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// 执行 Start 逻辑。
    /// </summary>
    private void Start()
    {
        if (canFade)
            StartCoroutine(FadeCo());

        ApplyRandomOffset();
        ApplyRandomRotation();

        if (autoDestroy)
            Destroy(gameObject, destroyDelay);
    }

    /// <summary>
    /// 执行 FadeCo 逻辑。
    /// </summary>
    private IEnumerator FadeCo()
    {
        Color targetColor = Color.white;

        while (targetColor.a > 0)
        {
            targetColor.a = targetColor.a - (fadeSpeed * Time.deltaTime);
            sr.color = targetColor;
            yield return null;
        }

        sr.color = targetColor;
    }

    /// <summary>
    /// 执行 ApplyRandomOffset 逻辑。
    /// </summary>
    private void ApplyRandomOffset()
    {
        if (!randomOffset)
            return;

        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);
        transform.position += new Vector3(xOffset, yOffset, 0f);
    }

    /// <summary>
    /// 执行 ApplyRandomRotation 逻辑。
    /// </summary>
    private void ApplyRandomRotation()
    {
        if (!randomRotation)
            return;

        float zRotation = Random.Range(minRotation, maxRotation);
        transform.Rotate(0, 0, zRotation);
    }
}


