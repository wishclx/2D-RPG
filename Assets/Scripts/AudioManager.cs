using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDatabaseSO audioDatabase;//音频数据库
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [Space]

    // 全局 SFX 音量因子（由 UI 控制，范围 0..1）
    private float globalSFXMultiplier = 1f;

    private Transform player;

    private AudioClip lastMusicPlayed;//上次播放的音乐
    private string currentBgmGroupName;
    private Coroutine currentBgmCo;
    [SerializeField] private bool bgmShouldPlay;

    private void Awake()
    {
        //单例模式，确保只有一个AudioManager实例存在
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return; // <- 必须立即返回，避免把静态 instance 指向已销毁的对象
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        CacheAudioSources();//缓存音频源
    }

    private void CacheAudioSources()
    {
        // 尝试按顺序获取子 AudioSource，确保 bgmSource 与 sfxSource 不同
        var sources = GetComponentsInChildren<AudioSource>(true);
        if (bgmSource == null && sources.Length > 0)
            bgmSource = sources[0];
        if (sfxSource == null)
        {
            if (sources.Length > 1)
                sfxSource = sources[1];
            else if (sources.Length == 1)
                sfxSource = sources[0];
        }

        if (bgmSource == null)
            bgmSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (bgmSource != null && bgmSource.isPlaying == false && bgmShouldPlay)
        {
            if (string.IsNullOrEmpty(currentBgmGroupName) == false)
                NextBGM(currentBgmGroupName);//如果当前音乐停止且bgmShouldPlay为true，则尝试切换到下一个音乐组
        }

        if (bgmSource != null && bgmSource.isPlaying && bgmShouldPlay == false)
            StopBGM();//如果当前音乐正在播放但bgmShouldPlay为false，则停止音乐
    }

    public void StartBGM(string musicGroup)
    {
        bgmShouldPlay = true;

        if (musicGroup == currentBgmGroupName)
            return;//如果当前正在播放的音乐组与请求的相同，则不执行任何操作

        NextBGM(musicGroup);
    }

    public void NextBGM(string musicGroup)//切换到下一个音乐组
    {
        bgmShouldPlay = true;
        currentBgmGroupName = musicGroup;

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);

        currentBgmCo = StartCoroutine(SwitchMusicCo(musicGroup));
    }

    public void StopBGM()
    {
        bgmShouldPlay = false;

        if (bgmSource != null)
            StartCoroutine(FadeVolumeCo(bgmSource, 0, 1f));//淡出当前音乐

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);
    }

    private IEnumerator SwitchMusicCo(string musicGroup)
    {
        AudioClipData data = audioDatabase.Get(musicGroup);
        if (data == null)
        {
            Debug.Log($"Music group {musicGroup} not found in database!");
            yield break;
        }

        AudioClip nextMusic = data.GetRandomClip();

        if (data.clips.Count > 1)
        {
            while (nextMusic != lastMusicPlayed)
                nextMusic = data.GetRandomClip();
        }

        if (bgmSource != null && bgmSource.isPlaying)
            yield return StartCoroutine(FadeVolumeCo(bgmSource, 0, 1f));//淡出当前音乐

        lastMusicPlayed = nextMusic;
        if (bgmSource != null)
        {
            bgmSource.clip = nextMusic;
            bgmSource.volume = 0;
            bgmSource.Play();

            StartCoroutine(FadeVolumeCo(bgmSource, data.MaxVolume, 1f));//淡入新音乐
        }
    }

    private IEnumerator FadeVolumeCo(AudioSource source, float targetVolume, float duration)
    {
        float time = 0;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;

            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);//修正为从 startVolume lerp 到 targetVolume
            yield return null;
        }

        source.volume = targetVolume;//确保最终音量设置正确
    }

    // 设置全局 SFX 因子（0..1），UI 调整时调用
    public void SetGlobalSFXMultiplier(float value)
    {
        globalSFXMultiplier = Mathf.Clamp01(value);
    }

    public void PlaySFX(string soundName, AudioSource sfxSource, float minDistanceToHearSound = 5)
    {
        if (player == null)
            player = Player.instance != null ? Player.instance.transform : null;

        var data = audioDatabase.Get(soundName);//从数据库中获取音频数据
        if (data == null)
        {
            Debug.Log($"Sound {soundName} not found in database!");
            return;
        }

        var clip = data.GetRandomClip();
        if (clip == null) return;

        float maxVolume = data.MaxVolume;
        float distance = (player != null && sfxSource != null) ? Vector2.Distance(sfxSource.transform.position, player.position) : 0f;
        float t = Mathf.Clamp01(1 - distance / minDistanceToHearSound);

        sfxSource.pitch = Random.Range(0.95f, 1.05f);//随机调整音调

        // 使用计算音量并乘以全局 SFX 因子，保证 UI 的调节生效
        float computedVolume = Mathf.Lerp(0, maxVolume, t * t) * globalSFXMultiplier;
        sfxSource.volume = computedVolume;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayGlobalSFX(string soundName)
    {
        if (sfxSource == null) //音效源已被销毁或未绑定
        {
            CacheAudioSources();
            if (sfxSource == null)
            {
                Debug.LogWarning("AudioManager: sfxSource为空或已被销毁，无法播放全局音效");
                return;
            }
        }

        var data = audioDatabase.Get(soundName);
        if (data == null) return;

        var clip = data.GetRandomClip();
        if (clip == null) return;

        Debug.Log($"Playing global SFX: {soundName}");
        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        // 使用资源最大音量并乘以全局因子
        sfxSource.volume = data.MaxVolume * globalSFXMultiplier;
        sfxSource.PlayOneShot(clip);//全局音效不受距离影响，直接使用最大音量播放
    }
}