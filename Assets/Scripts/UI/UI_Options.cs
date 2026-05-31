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

        // 绑定 slider 的回调（如果在 Inspector 没绑定的话）
        bgmSlider.onValueChanged.AddListener(BGMSliderValue);
        sfxSlider.onValueChanged.AddListener(SFXSliderValue);
    }

    public void BGMSliderValue(float value)
    {
        //将滑块值转换为音量值
        float newValue = Mathf.Log10(Mathf.Max(value, 0.0001f)) * mixerMultiplier;//将线性值转换为对数值，避免 log10(0)
        audioMixer.SetFloat(bgmParameter, newValue);
    }

    public void SFXSliderValue(float value)
    {
        //将滑块值转换为音量值
        float newValue = Mathf.Log10(Mathf.Max(value, 0.0001f)) * mixerMultiplier;//将线性值转换为对数值
        audioMixer.SetFloat(sfxParameter, newValue);

        // 额外：通知 AudioManager 更新全局 SFX 因子，使代码中直接设置音量的调用也受影响
        if (AudioManager.instance != null)
            AudioManager.instance.SetGlobalSFXMultiplier(value);
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

        // 同步到 AudioManager（确保播放时生效）
        if (AudioManager.instance != null)
            AudioManager.instance.SetGlobalSFXMultiplier(sfxSlider.value);
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

        if (AudioManager.instance != null)
            AudioManager.instance.SetGlobalSFXMultiplier(sfxSlider.value);
    }
}
