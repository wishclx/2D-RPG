using System.Collections;
using UnityEngine;

public class Player_VFX : Entity_VFX
{
    [Header("Image Echo VFX")]// 锥体拖影特效设置
    [Range(.01f, .2f)]
    [SerializeField] private float imageEchoInterval = .05f;
    [SerializeField] private GameObject imageEchoPrefab;// 拖影预制体引用
    private Coroutine imageEchoCo;

    public void CreateEffectOf(GameObject effect, Transform target)
    {
        Instantiate(effect, target.position, Quaternion.identity);//在目标位置创建特效实例，旋转为默认值
    }

    public void DoImageEchoEffect(float duration)
    {
        if (imageEchoCo != null)
            StopCoroutine(imageEchoCo);

        imageEchoCo = StartCoroutine(ImageEchoEffectCo(duration));
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
        GameObject imageEcho = Instantiate(imageEchoPrefab, transform.position, transform.rotation);

        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
    }
}


