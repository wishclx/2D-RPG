using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string musicGroupName;

    private void Start()
    {
        AudioManager.instance.StartBGM(musicGroupName);//在关卡开始时，调用AudioManager的StartBGM方法，传入指定的音乐组名称，以开始播放背景音乐
    }
}
