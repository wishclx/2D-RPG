using System.Collections;
using UnityEngine;

/// <summary>
/// Player_VFX 的职责说明。
/// </summary>
public class Player_VFX : Entity_VFX
{
    [Header("Image Echo VFX")]// 锥体拖影特效设置
    [Range(.01f, .2f)]
    [SerializeField] private float imageEchoInterval = .05f;
    [SerializeField] private GameObject imageEchoPrefab;// 拖影预制体引用
    private Coroutine imageEchoCo;

    /// <summary>
    /// 执行 DoImageEchoEffect 逻辑。
    /// </summary>
    public void DoImageEchoEffect(float duration)
    {
        if (imageEchoCo != null)
            StopCoroutine(imageEchoCo);

        imageEchoCo = StartCoroutine(ImageEchoEffectCo(duration));
    }

    /// <summary>
    /// 执行 ImageEchoEffectCo 逻辑。
    /// </summary>
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

    /// <summary>
    /// 执行 CreateImageEcho 逻辑。
    /// </summary>
    private void CreateImageEcho()
    {
        GameObject imageEcho = Instantiate(imageEchoPrefab, transform.position, transform.rotation);

        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
    }
}


