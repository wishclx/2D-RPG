using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;// 定义一个SpriteRenderer变量来存储组件引用
    private Entity entity;

    [Header("On Taking Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = .15f;
    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;

    [Header("On Doing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;// 定义一个GameObject变量来存储击中特效的预制体引用
    [SerializeField] private GameObject critHitVfx;// 定义一个GameObject变量来存储暴击击中特效的预制体引用

    [Header("Element Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;// 定义一个Color变量来存储冰冻特效的颜色
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color electrifyVfx = Color.yellow;
    private Color originalHitVfxColor;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();// 在Awake方法中获取SpriteRenderer组件的引用
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
    }

    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Ice)
            StartCoroutine(PlayStatusVfxCo(duration, chillVfx));// 如果元素类型是冰，则调用协程来播放冰冻状态特效，传入持续时间和冰冻特效颜色参数

        if (element == ElementType.Fire)
            StartCoroutine(PlayStatusVfxCo(duration, burnVfx));

        if (element == ElementType.Lightning)
            StartCoroutine(PlayStatusVfxCo(duration, electrifyVfx));
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

        Color lightColor = effectColor * 1.2f;// 计算一个更亮的颜色，用于闪烁效果
        Color darkColor = effectColor * 0.8f;// 计算一个更暗的颜色，用于闪烁效果

        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor;// 根据toggle的值切换颜色，实现闪烁效果
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);
            timeHasPassed = timeHasPassed + tickInterval;
        }

        sr.color = Color.white;
    }

    public void CreateOnHitVFX(Transform target, bool isCrit)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;// 根据是否暴击选择相应的击中特效预制体
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);// 在指定位置实例化击中特效预制体
        vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;// 设置击中特效的颜色

        if (entity.facingDir == -1 && isCrit)
            vfx.transform.Rotate(0, 180, 0);
    }

    public void UpdateOnHitColor(ElementType element)
    {
        if (element == ElementType.Ice)
            hitVfxColor = chillVfx;

        if (element == ElementType.None)
            hitVfxColor = originalHitVfxColor;
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine);// 如果当前正在播放受伤特效的协程不为null，则停止该协程，以确保不会同时播放多个受伤特效

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo()); // 调用协程来播放受伤特效 
    }

    private IEnumerator OnDamageVfxCo()// 定义一个协程方法来处理受伤特效的播放和恢复
    {
        sr.material = onDamageMaterial;

        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial;
    }

}
