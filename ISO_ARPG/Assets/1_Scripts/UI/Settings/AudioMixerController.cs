using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voicesSlider;

    private void Start()
    {
        InitializeSlider(masterSlider, "MasterSldr");
        InitializeSlider(musicSlider, "MusicSldr");
        InitializeSlider(sfxSlider, "SFXSldr");
        InitializeSlider(voicesSlider, "VoicesSldr");

        // Set up listeners for each slider
        masterSlider.onValueChanged.AddListener(value => SetMixerVolume("Master", value));
        musicSlider.onValueChanged.AddListener(value => SetMixerVolume("Music", value));
        sfxSlider.onValueChanged.AddListener(value => SetMixerVolume("SFX", value));
        voicesSlider.onValueChanged.AddListener(value => SetMixerVolume("Voices", value));
    }

    private void InitializeSlider(Slider slider, string parameterName)
    {
        // Get current mixer value and convert to slider value
        if (audioMixer.GetFloat(parameterName, out float dbValue))
        {
            // Convert dB to linear (0-1)
            float linearValue = Mathf.Pow(10, dbValue / 20f);
            slider.value = linearValue;
        }
        else
        {
            Debug.LogWarning($"AudioMixer parameter '{parameterName}' not found!");
        }
    }
    private void SetMixerVolume(string parameterName, float volume)
    {
        audioMixer.SetFloat(parameterName, volume);
    }
}