using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeScreen : MonoBehaviour
{
    public Coroutine fadeEffectCo { get; private set; }
    private Image fadeImage;//用于实现淡入淡出效果的UI图像组件

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 1);//在Awake方法中获取Image组件，并将其颜色设置为黑色且完全不透明，准备进行淡入淡出效果
    }

    public void DoFadeIn(float duration = 1) // 淡入
    {
        fadeImage.color = new Color(0, 0, 0, 1);//将图像的颜色设置为黑色，并且alpha值为1，表示完全不透明
        FadeEffect(0f, duration);
    }

    public void DoFadeOut(float duration = 1) // 淡出
    {
        fadeImage.color = new Color(0, 0, 0, 0);//将图像的颜色设置为黑色，并且alpha值为0，表示完全透明
        FadeEffect(1f, duration);
    }

    //淡入效果：将图像的alpha值从0逐渐增加到1，持续时间为duration秒
    private void FadeEffect(float targetAlpha, float duration)
    {
        if (fadeEffectCo != null)
            StopCoroutine(fadeEffectCo);

        fadeEffectCo = StartCoroutine(FadeEffectCo(targetAlpha, duration));
    }

    private IEnumerator FadeEffectCo(float targetAlpha, float duration)
    {
        float statrtAlpha = fadeImage.color.a;//获取当前图像的alpha值作为起始alpha
        float time = 0f;

        while (time < duration)
        {
            time = time + Time.deltaTime;

            var color = fadeImage.color;
            color.a = Mathf.Lerp(statrtAlpha, targetAlpha, time / duration);//使用线性插值函数计算当前alpha值，随着时间的推移逐渐接近目标alpha

            fadeImage.color = color;

            yield return null;//等待下一帧继续执行循环
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);//确保最终alpha值准确设置为目标alpha
    }
}
