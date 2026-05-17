using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    protected SpriteRenderer sr;
    private Entity entity;

    [Header("残影特效")]// 锥体拖影特效设置
    [Range(.01f, .2f)]
    [SerializeField] private float imageEchoInterval = .05f;
    [SerializeField] private GameObject imageEchoPrefab;// 拖影预制体引用
    private Coroutine imageEchoCo;

    [Header("受伤特效")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = .15f;
    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;

    [Header("攻击特效")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;

    [Header("元素颜色")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color shockVfx = Color.yellow;
    private Color originalHitVfxColor;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
    }

    public void DoImageEchoEffect(float duration)
    {
        StopImageEchoEffect();

        imageEchoCo = StartCoroutine(ImageEchoEffectCo(duration));
    }

    public void StopImageEchoEffect()
    {
        if (imageEchoCo != null)
            StopCoroutine(imageEchoCo);
    }

    private IEnumerator ImageEchoEffectCo(float duration)
    {
        float timeTracker = 0;

        while (timeTracker < duration)
        {
            CreateImageEcho();

            yield return new WaitForSeconds(imageEchoInterval);
            timeTracker += imageEchoInterval;
        }
    }

    private void CreateImageEcho()
    {
        Vector3 position = entity.anim.transform.position;//获取动画组件的位置作为拖影的生成位置
        float scale = entity.anim.transform.localScale.x;//获取动画组件的缩放作为拖影的缩放

        GameObject imageEcho = Instantiate(imageEchoPrefab, position, transform.rotation);

        imageEcho.transform.localScale = new Vector3(scale, scale, scale);//设置拖影的缩放与动画组件相同
        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
    }

    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Ice)
            StartCoroutine(PlayStatusVfxCo(duration, chillVfx));

        if (element == ElementType.Fire)
            StartCoroutine(PlayStatusVfxCo(duration, burnVfx));

        if (element == ElementType.Lightning)
            StartCoroutine(PlayStatusVfxCo(duration, shockVfx));
    }

    public void StopAllVfx()
    {
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;
    }

    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = .25f;
        float timeHasPassed = 0;

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * 0.8f;

        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);
            timeHasPassed = timeHasPassed + tickInterval;
        }

        sr.color = Color.white;
    }

    public void CreateOnHitVFX(Transform target, bool isCrit, ElementType element)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);
        //vfx.GetComponentInChildren<SpriteRenderer>().color = GetElementColor(element);//根据元素类型设置颜色

        if (entity.facingDir == -1 && isCrit)
            vfx.transform.Rotate(0, 180, 0);
    }

    public Color GetElementColor(ElementType element)
    {
        switch (element)
        {
            case ElementType.Ice:
                return chillVfx;
            case ElementType.Fire:
                return burnVfx;
            case ElementType.Lightning:
                return shockVfx;

            default:
                return Color.white;
        }
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine);

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;

        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial;
    }

}


