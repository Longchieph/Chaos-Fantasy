using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        // N?u ?� l?u gi� tr? �m l??ng, t?i ch�ng
        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("SFXVolume"))
        {
            LoadVolume();
        }
        else
        {
            // N?u kh�ng c� gi� tr? l?u, thi?t l?p �m l??ng m?c ??nh
            musicSlider.value = 0.5f;
            SFXSlider.value = 0.5f;
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    // Thi?t l?p �m l??ng nh?c
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20); // Chuy?n ??i sang decibel
        PlayerPrefs.SetFloat("musicVolume", volume); // L?u gi� tr?
    }

    // Thi?t l?p �m l??ng hi?u ?ng �m thanh
    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20); // Chuy?n ??i sang decibel
        PlayerPrefs.SetFloat("SFXVolume", volume); // L?u gi� tr?
    }

    // T?i c�c gi� tr? �m l??ng ?� l?u
    private void LoadVolume()
    {
        // ??t gi� tr? cho thanh tr??t v� �p d?ng �m l??ng
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        SetMusicVolume();
        SetSFXVolume();
    }
}
