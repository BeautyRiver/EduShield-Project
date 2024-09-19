using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;  // Master Audio 네임스페이스

public class VolumeController : MonoBehaviour
{
    public Slider bgmSlider;  // BGM 조절용 슬라이더
    public Slider sfxSlider;  // SFX 조절용 슬라이더

    private static readonly string BGM_VOLUME_KEY = "BGM_VOLUME";
    private static readonly string SFX_VOLUME_KEY = "SFX_VOLUME";

    void Start()
    {
        // 저장된 볼륨 값 불러오기, 없으면 기본값 1.0 사용
        float savedBGMVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1.0f);
        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1.0f);
        Debug.Log($"bgm : {savedBGMVolume} / sfx : {savedSFXVolume}");
        // 슬라이더 초기화
        bgmSlider.value = savedBGMVolume;  // BGM 현재 볼륨에 맞게 설정
        sfxSlider.value = savedSFXVolume;    // SFX 현재 볼륨에 맞게 설정

        // Master Audio의 초기 볼륨 설정
        MasterAudio.PlaylistMasterVolume = savedBGMVolume;
        MasterAudio.SetBusVolumeByName("SFX", savedSFXVolume);

        // 슬라이더 값 변경 시 이벤트 등록
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    void OnBgmVolumeChanged(float value)
    {
        // 슬라이더 값에 따라 BGM 볼륨 조절
        MasterAudio.PlaylistMasterVolume = value;
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        Debug.Log($"Saved bgm : {PlayerPrefs.GetFloat(BGM_VOLUME_KEY)}");

    }

    void OnSfxVolumeChanged(float value)
    {
        // 슬라이더 값에 따라 SFX 볼륨 조절
        MasterAudio.SetBusVolumeByName("SFX", value);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        Debug.Log($"Saved Sfx : {PlayerPrefs.GetFloat(SFX_VOLUME_KEY)}");

    }


}
