using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;  // Master Audio 네임스페이스

public class OptionManager : MonoBehaviour
{
    [SerializeField] private GameObject optionScreen;  // 옵션 화면
    [SerializeField] private Slider bgmSlider;  // BGM 조절용 슬라이더
    [SerializeField] private Slider sfxSlider;  // SFX 조절용 슬라이더
    private void Awake()
    {
        Slider[] sliders = optionScreen.GetComponentsInChildren<Slider>(true);
        bgmSlider = sliders[0];
        sfxSlider = sliders[1];
    }     
    void Start()
    {
        // 저장된 볼륨 값 불러오기, 없으면 기본값 0.5 사용
        float savedBGMVolume = PlayerPrefs.GetFloat("BGM", 0.5f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFX", 0.5f);
        // 슬라이더 초기화
        bgmSlider.value = savedBGMVolume;  // BGM 현재 볼륨에 맞게 설정
        sfxSlider.value = savedSFXVolume;    // SFX 현재 볼륨에 맞게 설정

        // Master Audio의 초기 볼륨 설정
        MasterAudio.PlaylistMasterVolume = savedBGMVolume;
        MasterAudio.MasterVolumeLevel = savedSFXVolume;

        // 슬라이더 값 변경 시 이벤트 등록
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    // BGM 볼륨 조절
    public void OnBgmVolumeChanged(float value)
    {
        
        // 슬라이더 값에 따라 BGM 볼륨 조절
        MasterAudio.PlaylistMasterVolume = value;
        PlayerPrefs.SetFloat("BGM", value);
    }

    // SFX 볼륨 조절
    public void OnSfxVolumeChanged(float value)
    {
        // 슬라이더 값에 따라 SFX 볼륨 조절
        MasterAudio.MasterVolumeLevel = value;
        PlayerPrefs.SetFloat("SFX", value);
    }

    public void ClosedOptionUI()
    {

    }

    // 게임이 종료되거나 씬이 변경되더라도 저장
    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();  // 저장된 값을 디스크에 기록
    }
}
