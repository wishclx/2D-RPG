using UnityEngine;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Player player;
    [SerializeField] private Toggle healthBarToggle;//血条显示开关

    private void Start()
    {
        player = FindFirstObjectByType<Player>();


        healthBarToggle.onValueChanged.AddListener(OnHealthBarToggleChanged);
    }

    private void OnHealthBarToggleChanged(bool isOn)
    {
        player.health.EnableHealthBar(isOn);//开关血条
    }

    public void GoMainMenuBTN() => GameManager.instance.ChangeScene("MainMenu", RespawnType.NoneSpecific);//切换到主菜单场景
}
