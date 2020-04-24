using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Inspired by the solution by FireTotemGames
/// </summary>
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private SliderType sliderType;

    private Slider _slider;

    void Start()
    {
        _slider = GetComponent<Slider>();

        switch (sliderType)
        {
            case SliderType.MusicVolume:
                _slider.SetValueWithoutNotify(AudioController.Instance.GetMusicVolume());
                break;
            case SliderType.SoundVolume:
                _slider.SetValueWithoutNotify(AudioController.Instance.GetSoundVolume());
                break;
        }
    }

    [UsedImplicitly]
    public void SetVolume(float value)
    {
        switch (sliderType)
        {
            case SliderType.MusicVolume:
                _slider.SetValueWithoutNotify(AudioController.Instance.SetMusicVolume(value));
                break;
            case SliderType.SoundVolume:
                _slider.SetValueWithoutNotify(AudioController.Instance.SetSoundVolume(value));
                break;
        }
    }

    private enum SliderType
    {
        MusicVolume,
        SoundVolume
    }
}