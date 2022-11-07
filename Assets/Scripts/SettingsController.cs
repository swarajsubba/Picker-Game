using System;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    private bool _musicState = true, _soundState = true;
    private int _on, _off;
    [SerializeField] private Animator musicSwitchAnimator, soundSwitchAnimator;

    private void Awake()
    {
        _on = Animator.StringToHash("On");
        _off = Animator.StringToHash("Off");

        if (!PlayerPrefs.HasKey("Music"))
            PlayerPrefs.SetInt("Music", 1);
        if (!PlayerPrefs.HasKey("Sound"))
            PlayerPrefs.SetInt("Sound", 1);

        SetAudioSettings();
    }

    private void SetAudioSettings()
    {
        _musicState = Convert.ToBoolean(PlayerPrefs.GetInt("Music"));
        _soundState = Convert.ToBoolean(PlayerPrefs.GetInt("Sound"));

        musicSwitchAnimator.Play(_musicState ? _on : _off);
        soundSwitchAnimator.Play(_soundState ? _on : _off);

        FindObjectOfType<AudioListener>().enabled = _soundState;
    }

    public void ToggleMusic()
    {
        _musicState = !_musicState;
        musicSwitchAnimator.Play(_musicState ? _on : _off);
        PlayerPrefs.SetInt("Music", Convert.ToInt32(_musicState));
    }

    public void ToggleSound()
    {
        _soundState = !_soundState;
        soundSwitchAnimator.Play(_soundState ? _on : _off);
        PlayerPrefs.SetInt("Sound", Convert.ToInt32(_soundState));

        FindObjectOfType<AudioListener>().enabled = _soundState;
    }
}
