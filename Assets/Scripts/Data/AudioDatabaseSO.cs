using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioDatabase")]
public class AudioDatabaseSO : ScriptableObject
{
    public List<AudioClipData> player;//玩家音频数据列表
    public List<AudioClipData> uiAudio;

    [Header("音频列表")]
    public List<AudioClipData> mainMenuMusic;//主菜单音乐数据列表
    public List<AudioClipData> levelMusic;

    private Dictionary<string, AudioClipData> clipCollection;//音频剪辑集合

    private void OnEnable()
    {
        //初始化音频剪辑集合
        clipCollection = new Dictionary<string, AudioClipData>();

        AddToCollection(player);
        AddToCollection(uiAudio);
        AddToCollection(mainMenuMusic);
        AddToCollection(levelMusic);
    }

    public AudioClipData Get(string groupName)
    {
        //尝试从集合中获取指定名称的音频剪辑数据，如果存在则返回，否则返回null
        return clipCollection.TryGetValue(groupName, out var data) ? data : null;
    }

    private void AddToCollection(List<AudioClipData> listToAdd)
    {
        foreach (var data in listToAdd)
        {
            //检查数据是否有效且名称不重复，然后添加到集合中
            if (data != null && clipCollection.ContainsKey(data.audioName) == false)
            {
                clipCollection.Add(data.audioName, data);
            }
        }
    }
}

[System.Serializable]
public class AudioClipData
{
    public string audioName;//音频名称
    public List<AudioClip> clips = new List<AudioClip>();//音频剪辑列表
    [Range(0f, 1f)] public float MaxVolume = 1f;//音量


    public AudioClip GetRandomClip()//随机获取一个音频剪辑
    {
        if (clips == null || clips.Count == 0)
        {
            Debug.LogWarning($"AudioClipData: {audioName} has no clips!");
            return null;
        }

        int index = Random.Range(0, clips.Count);
        return clips[index];
    }
}
