using UnityEngine;

public class Entity_SFX : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFX Names")]
    [SerializeField] private string attackHit;
    [SerializeField] private string attackMiss;
    [Space]
    [SerializeField] private float soundDistance = 15f;
    [SerializeField] private bool showGizmo;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void PlayAttackHit()
    {
        // 如果没有 AudioManager 或本对象未绑定 AudioSource，优雅处理避免 NullReferenceException
        if (AudioManager.instance == null)
            return;

        if (audioSource == null)
        {
            // 无本地音源时播放全局音效或忽略（根据需求选择）
            AudioManager.instance.PlayGlobalSFX(attackHit);
            return;
        }

        AudioManager.instance.PlaySFX(attackHit, audioSource, soundDistance);
    }

    public void PlayAttackMiss()
    {
        if (AudioManager.instance == null)
            return;

        if (audioSource == null)
        {
            AudioManager.instance.PlayGlobalSFX(attackMiss);
            return;
        }

        AudioManager.instance.PlaySFX(attackMiss, audioSource, soundDistance);
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmo)
        {
            //在编辑器中可视化音效范围
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, soundDistance);
        }
    }
}
