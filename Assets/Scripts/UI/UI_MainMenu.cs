using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    private void Start()
    {
        transform.root.GetComponentInChildren<UI_Options>(true).LoadUpVolume();//在主菜单界面开始时加载音量设置
        transform.root.GetComponentInChildren<UI_FadeScreen>().DoFadeIn();//在主菜单界面开始时执行淡入动画 

        AudioManager.instance.StartBGM("playlist_mainmenu");//开始播放主菜单音乐
    }

    public void PlayBTN()
    {
        AudioManager.instance.PlayGlobalSFX("button_click");//播放点击按钮的音效
        GameManager.instance.ContinuePlay();//继续游戏
    }

    public void QuitGameBTN()
    {
        Debug.Log("Quit Game Button Pressed");
        Application.Quit();//退出游戏
    }
}
