using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Player player;
    [SerializeField] private Toggle healthBarToggle;//血条显示开关

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25f;//音量调节乘数

    [Header("BGM设置")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private string bgmParameter;

    [Header("音效设置")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string sfxParameter;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();


        healthBarToggle.onValueChanged.AddListener(OnHealthBarToggleChanged);
    }

    public void BGMSliderValue(float value)
    {
        //将滑块值转换为音量值
        float newValue = Mathf.Log10(value) * mixerMultiplier;//将线性值转换为对数值
        audioMixer.SetFloat(bgmParameter, newValue);
    }

    public void SFXSliderValue(float value)
    {
        //将滑块值转换为音量值
        float newValue = Mathf.Log10(value) * mixerMultiplier;//将线性值转换为对数值
        audioMixer.SetFloat(sfxParameter, newValue);
    }

    private void OnHealthBarToggleChanged(bool isOn)
    {
        player.health.EnableHealthBar(isOn);//开关血条
    }

    public void GoMainMenuBTN() => GameManager.instance.ChangeScene("MainMenu", RespawnType.NoneSpecific);//切换到主菜单场景

    private void OnEnable()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, .6f);//从PlayerPrefs加载音效设置
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, .6f);//从PlayerPrefs加载BGM设置
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(sfxParameter, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParameter, bgmSlider.value);
    }

    public void LoadUpVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, .6f);//从PlayerPrefs加载音效设置
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, .6f);//从PlayerPrefs加载BGM设置
    }
}
