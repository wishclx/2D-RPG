using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDatabaseSO audioDatabase;//音频数据库
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [Space]


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
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        CacheAudioSources();//缓存音频源
    }

    private void CacheAudioSources()
    {
        if (bgmSource == null) //背景音乐音频源未绑定时尝试获取
            bgmSource = GetComponentInChildren<AudioSource>();

        if (sfxSource == null) //音效音频源未绑定时尝试获取
            sfxSource = GetComponentInChildren<AudioSource>();
    }

    private void Update()
    {
        if (bgmSource.isPlaying == false && bgmShouldPlay)
        {
            if (string.IsNullOrEmpty(currentBgmGroupName) == false)
                NextBGM(currentBgmGroupName);//如果当前音乐停止且bgmShouldPlay为true，则尝试切换到下一个音乐组
        }

        if (bgmSource.isPlaying && bgmShouldPlay == false)
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

        StartCoroutine(FadeVolumeCo(bgmSource, 0, 1f));//淡出当前音乐

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);
    }

    private IEnumerator SwitchMusicCo(string musicGroup)
    {
        AudioClipData data = audioDatabase.Get(musicGroup);
        AudioClip nextMusic = data.GetRandomClip();

        if (data == null || data.clips.Count == 0)
        {
            Debug.Log($"Music group {musicGroup} not found or empty in database!");
            yield break;
        }

        if (data.clips.Count > 1)
        {
            while (nextMusic != lastMusicPlayed)
                nextMusic = data.GetRandomClip();
        }

        if (bgmSource.isPlaying)
            yield return StartCoroutine(FadeVolumeCo(bgmSource, 0, 1f));//淡出当前音乐

        lastMusicPlayed = nextMusic;
        bgmSource.clip = nextMusic;
        bgmSource.volume = 0;
        bgmSource.Play();

        StartCoroutine(FadeVolumeCo(bgmSource, data.MaxVolume, 1f));//淡入新音乐
    }

    private IEnumerator FadeVolumeCo(AudioSource source, float targetVolume, float duration)
    {
        float time = 0;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;

            source.volume = Mathf.Lerp(source.volume, targetVolume, time / duration);//线性插值调整音量
            yield return null;
        }

        source.volume = targetVolume;//确保最终音量设置正确
    }

    public void PlaySFX(string soundName, AudioSource sfxSource, float minDistanceToHearSound = 5)
    {
        if (player == null)
            player = Player.instance.transform;

        var data = audioDatabase.Get(soundName);//从数据库中获取音频数据
        if (data == null)
        {
            Debug.Log($"Sound {soundName} not found in database!");
            return;
        }

        var clip = data.GetRandomClip();
        if (clip == null) return;

        float maxVolume = data.MaxVolume;
        float distance = Vector2.Distance(sfxSource.transform.position, player.position);
        float t = Mathf.Clamp01(1 - distance / minDistanceToHearSound);

        sfxSource.pitch = Random.Range(0.95f, 1.05f);//随机调整音调
        sfxSource.volume = Mathf.Lerp(0, maxVolume, t * t);//根据距离调整音量，距离越远音量越小
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
        sfxSource.volume = data.MaxVolume;
        sfxSource.PlayOneShot(clip);//全局音效不受距离影响，直接使用最大音量播放
    }
}