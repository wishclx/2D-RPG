using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    private void Start()
    {
        transform.root.GetComponentInChildren<UI_FadeScreen>().DoFadeIn();//在主菜单界面开始时执行淡入动画
    }

    public void PlayBTN()
    {
        Debug.Log("Play Button Pressed");
        GameManager.instance.ContinuePlay();//继续游戏
    }

    public void QuitGameBTN()
    {
        Debug.Log("Quit Game Button Pressed");
        Application.Quit();//退出游戏
    }
}
